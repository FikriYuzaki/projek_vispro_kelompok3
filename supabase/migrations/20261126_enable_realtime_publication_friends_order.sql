-- Proposed Migration (FOR REVIEW BY FIKRI — DO NOT APPLY DIRECTLY WITHOUT REVIEW)
-- Purpose:
--   Add Friends Order Room tables to the `supabase_realtime` publication so postgres_changes
--   events can stream to subscribed clients (respecting RLS policies on each table).
--
-- Tables proposed for `supabase_realtime`:
--   1. public.order_rooms   (room status changes such as open -> locked)
--   2. public.room_members  (new members joining via join_room_by_code)
--   3. public.orders        (parent group order creation and subtotal/total updates)
--   4. public.order_items   (items added/updated/deleted by room members)
--
-- Rollback / Risk notes:
--   - Idempotent: checks pg_publication_tables before calling ALTER PUBLICATION ... ADD TABLE.
--   - Supabase Realtime evaluates RLS per subscriber JWT; unauthorized users do not receive rows.
--   - Rollback: ALTER PUBLICATION supabase_realtime DROP TABLE public.order_rooms, public.room_members, public.orders, public.order_items;

do $$
declare
    v_table text;
    v_tables text[] := array['order_rooms', 'room_members', 'orders', 'order_items'];
begin
    if not exists (select 1 from pg_publication where pubname = 'supabase_realtime') then
        raise notice 'Publication supabase_realtime does not exist in this database; skipping.';
        return;
    end if;

    foreach v_table in array v_tables
    loop
        if not exists (
            select 1
            from pg_publication_tables
            where pubname = 'supabase_realtime'
              and schemaname = 'public'
              and tablename = v_table
        ) then
            execute format('alter publication supabase_realtime add table public.%I', v_table);
        end if;
    end loop;
end;
$$;
