-- Migration: Secure Friends Order Room RLS, Grants, and Server-Side Join RPC
-- Non-destructive and idempotent. Safe to run in Supabase SQL Editor or via Supabase CLI.

-- 1. Automatically register room host in room_members when a new room is created
create or replace function public.add_room_host_as_member()
returns trigger
language plpgsql
security definer
set search_path = public
as $$
declare
    v_display_name text;
begin
    select coalesce(nullif(trim(p.display_name), ''), 'Host')
    into v_display_name
    from public.profiles p
    where p.id = new.host_user_id;

    insert into public.room_members (room_id, user_id, display_name)
    values (new.id, new.host_user_id, coalesce(v_display_name, 'Host'))
    on conflict (room_id, user_id) do nothing;

    return new;
end;
$$;

drop trigger if exists trg_order_rooms_add_host_member on public.order_rooms;
create trigger trg_order_rooms_add_host_member
after insert on public.order_rooms
for each row execute function public.add_room_host_as_member();

-- Backfill existing room hosts into room_members without touching existing rows
insert into public.room_members (room_id, user_id, display_name)
select r.id, r.host_user_id, coalesce(nullif(trim(p.display_name), ''), 'Host')
from public.order_rooms r
left join public.profiles p on p.id = r.host_user_id
on conflict (room_id, user_id) do nothing;

-- 2. Server-side RPC to join an open room only when a valid invite code is supplied
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

-- 3. Restrict direct SELECT on order_rooms to host, room members, and staff only
drop policy if exists "order_rooms_member_or_staff_read" on public.order_rooms;
create policy "order_rooms_member_or_staff_read"
on public.order_rooms
for select
to authenticated
using (
    host_user_id = auth.uid()
    or public.is_room_member(id)
    or public.is_staff()
);

-- 4. Remove direct INSERT policy & table privilege on room_members so all joins go through join_room_by_code
drop policy if exists "room_members_self_join" on public.room_members;
revoke insert, update, delete on public.room_members from anon, authenticated;
grant select on public.room_members to authenticated;

-- 5. Function execution privileges
revoke all on function public.add_room_host_as_member() from public, anon, authenticated;
revoke all on function public.join_room_by_code(text) from public, anon;
grant execute on function public.join_room_by_code(text) to authenticated;
