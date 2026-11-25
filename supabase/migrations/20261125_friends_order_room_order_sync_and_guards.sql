-- Proposed Migration (FOR REVIEW BY FIKRI — DO NOT APPLY DIRECTLY WITHOUT REVIEW)
-- Purpose:
--   1. Enforce server-side that room order_items can only be inserted while order_rooms.status = 'open'
--      and that non-staff members can only insert items under their own member_user_id = auth.uid().
--   2. Automatically synchronize orders.subtotal and orders.total_amount via SECURITY DEFINER trigger
--      when order_items change, because orders UPDATE RLS is intentionally staff-only.
--   3. Return distinct RPC error messages in join_room_by_code for cancelled vs submitted vs locked rooms.
--
-- Rollback / Risk notes:
--   - Idempotent (CREATE OR REPLACE / DROP POLICY IF EXISTS).
--   - Does not alter existing table columns or drop data.
--   - Rollback: restore previous order_items_owner_or_room_insert policy and drop trg_order_items_sync_order_totals.

-- 1. Tighten order_items INSERT RLS so locked/cancelled/submitted rooms reject new items at DB level
--    and members cannot spoof another member's member_user_id.
drop policy if exists "order_items_owner_or_room_insert" on public.order_items;
create policy "order_items_owner_or_room_insert"
on public.order_items
for insert
to authenticated
with check (
    exists (
        select 1
        from public.orders o
        left join public.order_rooms r on r.id = o.room_id
        where o.id = order_id
          and o.status = 'Waiting'
          and (
              public.is_staff()
              or (
                  o.room_id is null
                  and o.customer_id = auth.uid()
                  and (member_user_id is null or member_user_id = auth.uid())
              )
              or (
                  o.room_id is not null
                  and r.status = 'open'
                  and public.is_room_member(o.room_id)
                  and member_user_id = auth.uid()
              )
          )
    )
);

-- 2. Keep orders.subtotal and orders.total_amount consistent when order_items change
--    without granting direct orders UPDATE privilege to customers.
create or replace function public.sync_order_totals_from_items()
returns trigger
language plpgsql
security definer
set search_path = public
as $$
declare
    v_order_id uuid;
    v_subtotal numeric(12,2);
begin
    v_order_id := coalesce(new.order_id, old.order_id);
    if v_order_id is null then
        return coalesce(new, old);
    end if;

    select coalesce(sum(oi.line_total), 0)
    into v_subtotal
    from public.order_items oi
    where oi.order_id = v_order_id;

    update public.orders o
    set subtotal = v_subtotal,
        total_amount = greatest(0, v_subtotal - coalesce(o.discount_amount, 0))
    where o.id = v_order_id;

    return coalesce(new, old);
end;
$$;

drop trigger if exists trg_order_items_sync_order_totals on public.order_items;
create trigger trg_order_items_sync_order_totals
after insert or update or delete on public.order_items
for each row execute function public.sync_order_totals_from_items();

revoke all on function public.sync_order_totals_from_items() from public, anon, authenticated;

-- 3. Distinguish cancelled and submitted rooms in join_room_by_code error messages
create or replace function public.join_room_by_code(p_room_code text)
returns table (
    room_id uuid,
    title text,
    status text,
    joined_at timestamptz
)
language plpgsql
security definer
set search_path = public
as $$
#variable_conflict use_column
declare
    v_user_id uuid;
    v_display_name text;
    v_normalized_code text;
    v_room record;
    v_joined_at timestamptz;
begin
    v_user_id := auth.uid();
    if v_user_id is null then
        raise exception 'Authentication required to join a room'
            using errcode = '42501';
    end if;

    select coalesce(nullif(trim(p.display_name), ''), 'Pelanggan')
    into v_display_name
    from public.profiles p
    where p.id = v_user_id;

    if not found then
        raise exception 'Authenticated user profile not found'
            using errcode = '42501';
    end if;

    v_normalized_code := upper(trim(coalesce(p_room_code, '')));
    if char_length(v_normalized_code) < 4 or char_length(v_normalized_code) > 16 then
        raise exception 'Invalid or unavailable room code'
            using errcode = 'P0001';
    end if;

    select r.id, r.title, r.status
    into v_room
    from public.order_rooms r
    where upper(trim(r.room_code)) = v_normalized_code
    limit 1;

    if not found then
        raise exception 'Invalid or unavailable room code'
            using errcode = 'P0001';
    end if;

    if v_room.status = 'locked' then
        raise exception 'Room is locked and no longer accepting members'
            using errcode = 'P0001';
    end if;

    if v_room.status = 'cancelled' then
        raise exception 'Room has been cancelled'
            using errcode = 'P0001';
    end if;

    if v_room.status = 'submitted' then
        raise exception 'Room order has already been submitted'
            using errcode = 'P0001';
    end if;

    if v_room.status <> 'open' then
        raise exception 'Room is no longer available'
            using errcode = 'P0001';
    end if;

    insert into public.room_members (room_id, user_id, display_name)
    values (v_room.id, v_user_id, v_display_name)
    on conflict (room_id, user_id) do nothing;

    select m.joined_at
    into v_joined_at
    from public.room_members m
    where m.room_id = v_room.id
      and m.user_id = v_user_id;

    return query
    select v_room.id, v_room.title, v_room.status, v_joined_at;
end;
$$;

revoke all on function public.join_room_by_code(text) from public, anon;
grant execute on function public.join_room_by_code(text) to authenticated;

