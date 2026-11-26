-- Proposed Migration (FOR REVIEW BY FIKRI — DO NOT APPLY DIRECTLY WITHOUT REVIEW)
-- Prerequisite: supabase/migrations/20261119_secure_order_rooms_rls_and_rpc.sql must be applied first.
--
-- Purpose:
--   1. Enforce single parent order per room via unique partial index on orders(room_id).
--   2. Tighten order_items INSERT RLS so room items can only be inserted while order_rooms.status = 'open'
--      and non-staff members can only insert items under their own member_user_id = auth.uid().
--   3. Enforce server-side pricing, menu availability, option ownership/availability, and line_total
--      recalculation on order_items (BEFORE INSERT OR UPDATE) so client-supplied prices cannot be tampered with.
--   4. Synchronize orders.subtotal and orders.total_amount on INSERT, UPDATE (including order_id reassignment),
--      and DELETE of order_items without modifying orders.status or breaking Live Brew Tracker.
--   5. Prevent join vs lock race condition in join_room_by_code using SELECT ... FOR UPDATE,
--      and return distinct errors for locked, cancelled, and submitted rooms.
--
-- Rollback / Risk notes:
--   - Non-destructive and idempotent (CREATE OR REPLACE / DROP IF EXISTS / CREATE UNIQUE INDEX IF NOT EXISTS).
--   - Does not modify existing columns, drop data, or weaken RLS.
--   - Does not touch orders.status or trg_orders_status_transition (Live Brew Tracker unaffected).
--   - Rollback: drop triggers trg_order_items_enforce_pricing & trg_order_items_sync_order_totals,
--     drop unique index uq_orders_single_parent_per_room, and restore prior policy/RPC definitions.

-- ============================================================================
-- 1. Single parent order per room (prevents duplicate parent orders under race)
-- ============================================================================
create unique index if not exists uq_orders_single_parent_per_room
    on public.orders (room_id)
    where room_id is not null;

-- ============================================================================
-- 2. Tighten order_items INSERT RLS (room must be open + member_user_id = auth.uid())
-- ============================================================================
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

-- ============================================================================
-- 3. Server-side price, option, and availability validation on order_items
--    Never trusts client unit_base_price, options_extra_price, line_total, or options_snapshot prices.
-- ============================================================================
create or replace function public.enforce_order_item_pricing_and_options()
returns trigger
language plpgsql
security definer
set search_path = public
as $$
declare
    v_menu record;
    v_opt record;
    v_raw_options jsonb;
    v_elem jsonb;
    v_opt_id_text text;
    v_opt_id uuid;
    v_seen_ids uuid[] := '{}'::uuid[];
    v_seen_single_groups text[] := '{}'::text[];
    v_canonical_options jsonb := '[]'::jsonb;
    v_options_extra numeric(12,2) := 0;
begin
    -- Validate quantity
    if new.quantity is null or new.quantity <= 0 or new.quantity > 99 then
        raise exception 'Invalid item quantity: must be between 1 and 99'
            using errcode = 'P0001';
    end if;

    -- Validate menu_item exists and is available
    select m.id, m.name, m.base_price, m.is_available, m.supports_customization
    into v_menu
    from public.menu_items m
    where m.id = new.menu_item_id;

    if not found then
        raise exception 'Menu item not found'
            using errcode = 'P0001';
    end if;

    if not v_menu.is_available then
        raise exception 'Menu item "%" is currently unavailable', v_menu.name
            using errcode = 'P0001';
    end if;

    -- Normalize options_snapshot: allow null or empty object '{}' as empty array '[]'
    v_raw_options := coalesce(new.options_snapshot, '[]'::jsonb);
    if v_raw_options = '{}'::jsonb then
        v_raw_options := '[]'::jsonb;
    end if;

    if jsonb_typeof(v_raw_options) <> 'array' then
        raise exception 'Invalid options_snapshot format: expected JSON array'
            using errcode = 'P0001';
    end if;

    if jsonb_array_length(v_raw_options) > 0 and not v_menu.supports_customization then
        raise exception 'Menu item "%" does not support customization options', v_menu.name
            using errcode = 'P0001';
    end if;

    -- Validate each option against menu_options and rebuild canonical snapshot from DB
    for v_elem in select * from jsonb_array_elements(v_raw_options)
    loop
        if jsonb_typeof(v_elem) <> 'object' then
            raise exception 'Invalid option entry in options_snapshot'
                using errcode = 'P0001';
        end if;

        v_opt_id_text := trim(coalesce(v_elem ->> 'option_id', ''));
        if v_opt_id_text = '' then
            raise exception 'Missing option_id in options_snapshot'
                using errcode = 'P0001';
        end if;

        begin
            v_opt_id := v_opt_id_text::uuid;
        exception when invalid_text_representation then
            raise exception 'Invalid option_id UUID: %', v_opt_id_text
                using errcode = 'P0001';
        end;

        if v_opt_id = any(v_seen_ids) then
            raise exception 'Duplicate option_id "%" is not allowed', v_opt_id
                using errcode = 'P0001';
        end if;
        v_seen_ids := array_append(v_seen_ids, v_opt_id);

        select o.id, o.menu_item_id, o.option_group, o.option_name, o.extra_price, o.is_available
        into v_opt
        from public.menu_options o
        where o.id = v_opt_id;

        if not found then
            raise exception 'Menu option "%" not found', v_opt_id
                using errcode = 'P0001';
        end if;

        if v_opt.menu_item_id <> new.menu_item_id then
            raise exception 'Option "%" does not belong to the selected menu item', v_opt.option_name
                using errcode = 'P0001';
        end if;

        if not v_opt.is_available then
            raise exception 'Option "%" is no longer available', v_opt.option_name
                using errcode = 'P0001';
        end if;

        -- Single-select groups (all except topping) may appear at most once per item
        if lower(v_opt.option_group) <> 'topping' then
            if lower(v_opt.option_group) = any(v_seen_single_groups) then
                raise exception 'Multiple options selected for single-select group "%"', v_opt.option_group
                    using errcode = 'P0001';
            end if;
            v_seen_single_groups := array_append(v_seen_single_groups, lower(v_opt.option_group));
        end if;

        v_options_extra := v_options_extra + v_opt.extra_price;
        v_canonical_options := v_canonical_options || jsonb_build_array(
            jsonb_build_object(
                'option_group', v_opt.option_group,
                'option_id', v_opt.id,
                'option_name', v_opt.option_name,
                'extra_price', v_opt.extra_price
            )
        );
    end loop;

    -- Overwrite snapshot and monetary fields with authoritative server-calculated values
    new.item_name_snapshot := v_menu.name;
    new.unit_base_price := v_menu.base_price;
    new.options_snapshot := v_canonical_options;
    new.options_extra_price := v_options_extra;
    new.line_total := (v_menu.base_price + v_options_extra) * new.quantity;

    return new;
end;
$$;

drop trigger if exists trg_order_items_enforce_pricing on public.order_items;
create trigger trg_order_items_enforce_pricing
before insert or update on public.order_items
for each row execute function public.enforce_order_item_pricing_and_options();

revoke all on function public.enforce_order_item_pricing_and_options() from public, anon, authenticated;

-- ============================================================================
-- 4. Keep orders.subtotal and orders.total_amount consistent on INSERT/UPDATE/DELETE
--    Handles order_id reassignment on UPDATE and preserves Live Brew Tracker status flow.
-- ============================================================================
create or replace function public.recalculate_order_totals(p_order_id uuid)
returns void
language plpgsql
security definer
set search_path = public
as $$
declare
    v_subtotal numeric(12,2);
begin
    if p_order_id is null then
        return;
    end if;

    select coalesce(sum(oi.line_total), 0)
    into v_subtotal
    from public.order_items oi
    where oi.order_id = p_order_id;

    -- Only updates monetary columns; never modifies status or triggers trg_orders_status_transition.
    update public.orders o
    set subtotal = v_subtotal,
        total_amount = greatest(0, v_subtotal - coalesce(o.discount_amount, 0))
    where o.id = p_order_id
      and (
          o.subtotal is distinct from v_subtotal
          or o.total_amount is distinct from greatest(0, v_subtotal - coalesce(o.discount_amount, 0))
      );
end;
$$;

revoke all on function public.recalculate_order_totals(uuid) from public, anon, authenticated;

create or replace function public.sync_order_totals_from_items()
returns trigger
language plpgsql
security definer
set search_path = public
as $$
begin
    if tg_op = 'INSERT' then
        perform public.recalculate_order_totals(new.order_id);
        return new;
    elsif tg_op = 'DELETE' then
        perform public.recalculate_order_totals(old.order_id);
        return old;
    elsif tg_op = 'UPDATE' then
        if old.order_id is distinct from new.order_id then
            perform public.recalculate_order_totals(old.order_id);
        end if;
        perform public.recalculate_order_totals(new.order_id);
        return new;
    end if;

    return coalesce(new, old);
end;
$$;

drop trigger if exists trg_order_items_sync_order_totals on public.order_items;
create trigger trg_order_items_sync_order_totals
after insert or update or delete on public.order_items
for each row execute function public.sync_order_totals_from_items();

revoke all on function public.sync_order_totals_from_items() from public, anon, authenticated;

-- Also keep total_amount consistent if discount_amount is updated directly by staff
create or replace function public.sync_order_total_on_discount_change()
returns trigger
language plpgsql
security definer
set search_path = public
as $$
begin
    new.total_amount := greatest(0, coalesce(new.subtotal, 0) - coalesce(new.discount_amount, 0));
    return new;
end;
$$;

drop trigger if exists trg_orders_sync_total_on_discount on public.orders;
create trigger trg_orders_sync_total_on_discount
before update of discount_amount on public.orders
for each row execute function public.sync_order_total_on_discount_change();

revoke all on function public.sync_order_total_on_discount_change() from public, anon, authenticated;

-- ============================================================================
-- 5. Race-safe join_room_by_code using SELECT ... FOR UPDATE row lock
--    Serializes concurrent join vs host lock and distinguishes cancelled/submitted/locked.
-- ============================================================================
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

    -- Acquire transactional row lock on order_rooms to prevent concurrent join + host lock race
    select r.id, r.title, r.status
    into v_room
    from public.order_rooms r
    where upper(trim(r.room_code)) = v_normalized_code
    for update;

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
