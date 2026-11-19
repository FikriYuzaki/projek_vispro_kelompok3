-- CuppaClub Shared Supabase PostgreSQL Schema & Row Level Security
-- Single source of truth for Features 01 through 05.
-- Safe to run multiple times in Supabase SQL Editor.

create extension if not exists pgcrypto;

-- 1. Helper trigger function for updated_at timestamps
create or replace function public.set_updated_at()
returns trigger
language plpgsql
as $$
begin
    new.updated_at = now();
    return new;
end;
$$;

-- 2. User profiles and role management
create table if not exists public.profiles (
    id uuid primary key references auth.users(id) on delete cascade,
    display_name text not null default '',
    email text not null default '',
    role text not null default 'customer' check (role in ('customer', 'staff', 'admin')),
    created_at timestamptz not null default now(),
    updated_at timestamptz not null default now()
);

drop trigger if exists trg_profiles_updated_at on public.profiles;
create trigger trg_profiles_updated_at
before update on public.profiles
for each row execute function public.set_updated_at();

-- Always assign 'customer' on signup so clients cannot self-escalate roles.
-- Promote staff accounts via SQL Editor or Supabase Dashboard only.
create or replace function public.handle_new_user()
returns trigger
language plpgsql
security definer
set search_path = public
as $$
declare
    v_display_name text;
begin
    v_display_name := coalesce(
        nullif(trim(new.raw_user_meta_data ->> 'display_name'), ''),
        nullif(split_part(coalesce(new.email, ''), '@', 1), ''),
        'Pelanggan'
    );

    insert into public.profiles (id, display_name, email, role)
    values (new.id, v_display_name, coalesce(new.email, ''), 'customer')
    on conflict (id) do update
        set email = excluded.email,
            display_name = case
                when trim(public.profiles.display_name) = '' then excluded.display_name
                else public.profiles.display_name
            end;

    return new;
end;
$$;

drop trigger if exists on_auth_user_created on auth.users;
create trigger on_auth_user_created
after insert on auth.users
for each row execute function public.handle_new_user();

create or replace function public.is_staff()
returns boolean
language sql
stable
security definer
set search_path = public
as $$
    select exists (
        select 1
        from public.profiles p
        where p.id = auth.uid()
          and p.role in ('staff', 'admin')
    );
$$;

alter table public.profiles enable row level security;

drop policy if exists "profiles_select_self_or_staff" on public.profiles;
create policy "profiles_select_self_or_staff"
on public.profiles
for select
to authenticated
using (auth.uid() = id or public.is_staff());

drop policy if exists "profiles_update_self_display_name" on public.profiles;
create policy "profiles_update_self_display_name"
on public.profiles
for update
to authenticated
using (auth.uid() = id)
with check (
    auth.uid() = id
    and role = (select p.role from public.profiles p where p.id = auth.uid())
);

-- 3. Feature 01: Cafe Menu Gallery
create table if not exists public.menu_items (
    id uuid primary key default gen_random_uuid(),
    name text not null check (char_length(trim(name)) > 0),
    category text not null check (category in ('Coffee', 'Non-Coffee', 'Pastry', 'Dessert')),
    description text not null default '',
    base_price numeric(12,2) not null check (base_price >= 0),
    image_url text not null default '',
    is_available boolean not null default true,
    supports_customization boolean not null default false,
    created_at timestamptz not null default now(),
    updated_at timestamptz not null default now()
);

create index if not exists idx_menu_items_category_available
    on public.menu_items (category, is_available);

drop trigger if exists trg_menu_items_updated_at on public.menu_items;
create trigger trg_menu_items_updated_at
before update on public.menu_items
for each row execute function public.set_updated_at();

alter table public.menu_items enable row level security;

drop policy if exists "menu_items_public_read" on public.menu_items;
create policy "menu_items_public_read"
on public.menu_items
for select
to anon, authenticated
using (true);

drop policy if exists "menu_items_staff_insert" on public.menu_items;
create policy "menu_items_staff_insert"
on public.menu_items
for insert
to authenticated
with check (public.is_staff());

drop policy if exists "menu_items_staff_update" on public.menu_items;
create policy "menu_items_staff_update"
on public.menu_items
for update
to authenticated
using (public.is_staff())
with check (public.is_staff());

drop policy if exists "menu_items_staff_delete" on public.menu_items;
create policy "menu_items_staff_delete"
on public.menu_items
for delete
to authenticated
using (public.is_staff());

-- 4. Feature 02: My Drink Creator (structured customization options per menu item)
create table if not exists public.menu_options (
    id uuid primary key default gen_random_uuid(),
    menu_item_id uuid not null references public.menu_items(id) on delete cascade,
    option_group text not null check (option_group in ('size', 'sugar', 'ice', 'milk', 'topping', 'espresso')),
    option_name text not null check (char_length(trim(option_name)) > 0),
    extra_price numeric(12,2) not null default 0 check (extra_price >= 0),
    is_default boolean not null default false,
    is_available boolean not null default true,
    created_at timestamptz not null default now()
);

create index if not exists idx_menu_options_item_group
    on public.menu_options (menu_item_id, option_group);

alter table public.menu_options enable row level security;

drop policy if exists "menu_options_public_read" on public.menu_options;
create policy "menu_options_public_read"
on public.menu_options
for select
to anon, authenticated
using (true);

drop policy if exists "menu_options_staff_write" on public.menu_options;
create policy "menu_options_staff_write"
on public.menu_options
for all
to authenticated
using (public.is_staff())
with check (public.is_staff());

-- 5. Feature 03: Friends Order Room
create table if not exists public.order_rooms (
    id uuid primary key default gen_random_uuid(),
    room_code text not null unique check (char_length(trim(room_code)) between 4 and 16),
    title text not null default '',
    host_user_id uuid not null references public.profiles(id) on delete restrict,
    status text not null default 'open' check (status in ('open', 'locked', 'submitted', 'cancelled')),
    created_at timestamptz not null default now(),
    locked_at timestamptz,
    submitted_at timestamptz
);

create table if not exists public.room_members (
    id uuid primary key default gen_random_uuid(),
    room_id uuid not null references public.order_rooms(id) on delete cascade,
    user_id uuid not null references public.profiles(id) on delete cascade,
    display_name text not null default '',
    joined_at timestamptz not null default now(),
    unique (room_id, user_id)
);

create or replace function public.is_room_member(p_room_id uuid)
returns boolean
language sql
stable
security definer
set search_path = public
as $$
    select exists (
        select 1
        from public.order_rooms r
        where r.id = p_room_id
          and r.host_user_id = auth.uid()
    )
    or exists (
        select 1
        from public.room_members m
        where m.room_id = p_room_id
          and m.user_id = auth.uid()
    );
$$;

alter table public.order_rooms enable row level security;
alter table public.room_members enable row level security;

drop policy if exists "order_rooms_member_or_staff_read" on public.order_rooms;
create policy "order_rooms_member_or_staff_read"
on public.order_rooms
for select
to authenticated
using (public.is_room_member(id) or public.is_staff() or status = 'open');

drop policy if exists "order_rooms_host_insert" on public.order_rooms;
create policy "order_rooms_host_insert"
on public.order_rooms
for insert
to authenticated
with check (host_user_id = auth.uid());

drop policy if exists "order_rooms_host_or_staff_update" on public.order_rooms;
create policy "order_rooms_host_or_staff_update"
on public.order_rooms
for update
to authenticated
using (host_user_id = auth.uid() or public.is_staff())
with check (host_user_id = auth.uid() or public.is_staff());

drop policy if exists "room_members_member_or_staff_read" on public.room_members;
create policy "room_members_member_or_staff_read"
on public.room_members
for select
to authenticated
using (public.is_room_member(room_id) or public.is_staff());

drop policy if exists "room_members_self_join" on public.room_members;
create policy "room_members_self_join"
on public.room_members
for insert
to authenticated
with check (
    user_id = auth.uid()
    and exists (
        select 1
        from public.order_rooms r
        where r.id = room_id
          and r.status = 'open'
    )
);

-- 6. Feature 04: Orders & Live Brew Tracker
create table if not exists public.orders (
    id uuid primary key default gen_random_uuid(),
    order_number text not null unique,
    customer_id uuid references public.profiles(id) on delete restrict,
    room_id uuid references public.order_rooms(id) on delete set null,
    status text not null default 'Waiting' check (status in ('Waiting', 'Preparing', 'Ready', 'Completed')),
    payment_method text not null default 'cashier_sim',
    payment_status text not null default 'pending' check (payment_status in ('pending', 'paid_sim', 'cancelled')),
    subtotal numeric(12,2) not null default 0 check (subtotal >= 0),
    discount_amount numeric(12,2) not null default 0 check (discount_amount >= 0),
    total_amount numeric(12,2) not null default 0 check (total_amount >= 0),
    created_at timestamptz not null default now(),
    updated_at timestamptz not null default now()
);

create index if not exists idx_orders_status_created on public.orders (status, created_at desc);
create index if not exists idx_orders_customer_created on public.orders (customer_id, created_at desc);

drop trigger if exists trg_orders_updated_at on public.orders;
create trigger trg_orders_updated_at
before update on public.orders
for each row execute function public.set_updated_at();

create table if not exists public.order_items (
    id uuid primary key default gen_random_uuid(),
    order_id uuid not null references public.orders(id) on delete cascade,
    menu_item_id uuid not null references public.menu_items(id) on delete restrict,
    member_user_id uuid references public.profiles(id) on delete set null,
    item_name_snapshot text not null,
    unit_base_price numeric(12,2) not null check (unit_base_price >= 0),
    options_snapshot jsonb not null default '{}'::jsonb,
    options_extra_price numeric(12,2) not null default 0 check (options_extra_price >= 0),
    quantity integer not null check (quantity > 0),
    line_total numeric(12,2) not null check (line_total >= 0),
    notes text not null default '',
    created_at timestamptz not null default now()
);

create index if not exists idx_order_items_order on public.order_items (order_id);

create table if not exists public.order_status_history (
    id uuid primary key default gen_random_uuid(),
    order_id uuid not null references public.orders(id) on delete cascade,
    previous_status text,
    new_status text not null check (new_status in ('Waiting', 'Preparing', 'Ready', 'Completed')),
    changed_by uuid references public.profiles(id) on delete set null,
    changed_at timestamptz not null default now()
);

create index if not exists idx_order_status_history_order on public.order_status_history (order_id, changed_at desc);

create or replace function public.enforce_order_status_transition()
returns trigger
language plpgsql
security definer
set search_path = public
as $$
begin
    if tg_op = 'INSERT' then
        insert into public.order_status_history (order_id, previous_status, new_status, changed_by)
        values (new.id, null, new.status, coalesce(auth.uid(), new.customer_id));
        return new;
    end if;

    if new.status is distinct from old.status then
        if not (
            (old.status = 'Waiting' and new.status = 'Preparing') or
            (old.status = 'Preparing' and new.status = 'Ready') or
            (old.status = 'Ready' and new.status = 'Completed')
        ) then
            raise exception 'Invalid order status transition from % to %', old.status, new.status;
        end if;

        insert into public.order_status_history (order_id, previous_status, new_status, changed_by)
        values (new.id, old.status, new.status, auth.uid());
    end if;

    return new;
end;
$$;

drop trigger if exists trg_orders_status_transition on public.orders;
create trigger trg_orders_status_transition
after insert or update of status on public.orders
for each row execute function public.enforce_order_status_transition();

alter table public.orders enable row level security;
alter table public.order_items enable row level security;
alter table public.order_status_history enable row level security;

drop policy if exists "orders_owner_room_or_staff_read" on public.orders;
create policy "orders_owner_room_or_staff_read"
on public.orders
for select
to authenticated
using (
    customer_id = auth.uid()
    or (room_id is not null and public.is_room_member(room_id))
    or public.is_staff()
);

drop policy if exists "orders_authenticated_insert" on public.orders;
create policy "orders_authenticated_insert"
on public.orders
for insert
to authenticated
with check (customer_id = auth.uid() or public.is_staff());

drop policy if exists "orders_staff_update" on public.orders;
create policy "orders_staff_update"
on public.orders
for update
to authenticated
using (public.is_staff())
with check (public.is_staff());

drop policy if exists "order_items_owner_room_or_staff_read" on public.order_items;
create policy "order_items_owner_room_or_staff_read"
on public.order_items
for select
to authenticated
using (
    exists (
        select 1
        from public.orders o
        where o.id = order_id
          and (
              o.customer_id = auth.uid()
              or (o.room_id is not null and public.is_room_member(o.room_id))
              or public.is_staff()
          )
    )
);

drop policy if exists "order_items_owner_or_room_insert" on public.order_items;
create policy "order_items_owner_or_room_insert"
on public.order_items
for insert
to authenticated
with check (
    exists (
        select 1
        from public.orders o
        where o.id = order_id
          and o.status = 'Waiting'
          and (
              o.customer_id = auth.uid()
              or (o.room_id is not null and public.is_room_member(o.room_id))
              or public.is_staff()
          )
    )
);

drop policy if exists "order_status_history_read" on public.order_status_history;
create policy "order_status_history_read"
on public.order_status_history
for select
to authenticated
using (
    exists (
        select 1
        from public.orders o
        where o.id = order_id
          and (
              o.customer_id = auth.uid()
              or (o.room_id is not null and public.is_room_member(o.room_id))
              or public.is_staff()
          )
    )
);

-- 7. Feature 05: Sweet Rewards Club
create table if not exists public.reward_catalog (
    id uuid primary key default gen_random_uuid(),
    title text not null check (char_length(trim(title)) > 0),
    description text not null default '',
    points_cost integer not null check (points_cost > 0),
    is_available boolean not null default true,
    stock_qty integer check (stock_qty is null or stock_qty >= 0),
    created_at timestamptz not null default now(),
    updated_at timestamptz not null default now()
);

drop trigger if exists trg_reward_catalog_updated_at on public.reward_catalog;
create trigger trg_reward_catalog_updated_at
before update on public.reward_catalog
for each row execute function public.set_updated_at();

create table if not exists public.reward_transactions (
    id uuid primary key default gen_random_uuid(),
    user_id uuid not null references public.profiles(id) on delete cascade,
    order_id uuid references public.orders(id) on delete set null,
    reward_id uuid references public.reward_catalog(id) on delete set null,
    tx_type text not null check (tx_type in ('earn', 'redeem', 'adjust')),
    points_delta integer not null check (points_delta <> 0),
    description text not null default '',
    created_at timestamptz not null default now()
);

create index if not exists idx_reward_tx_user_created on public.reward_transactions (user_id, created_at desc);
create unique index if not exists uq_reward_earn_per_order
    on public.reward_transactions (order_id)
    where tx_type = 'earn' and order_id is not null;

alter table public.reward_catalog enable row level security;
alter table public.reward_transactions enable row level security;

drop policy if exists "reward_catalog_read" on public.reward_catalog;
create policy "reward_catalog_read"
on public.reward_catalog
for select
to anon, authenticated
using (true);

drop policy if exists "reward_catalog_staff_write" on public.reward_catalog;
create policy "reward_catalog_staff_write"
on public.reward_catalog
for all
to authenticated
using (public.is_staff())
with check (public.is_staff());

drop policy if exists "reward_transactions_self_or_staff_read" on public.reward_transactions;
create policy "reward_transactions_self_or_staff_read"
on public.reward_transactions
for select
to authenticated
using (user_id = auth.uid() or public.is_staff());

-- 8. Storage bucket for menu item photos
insert into storage.buckets (id, name, public)
values ('menu-photos', 'menu-photos', true)
on conflict (id) do nothing;

drop policy if exists "menu_photos_public_read" on storage.objects;
create policy "menu_photos_public_read"
on storage.objects
for select
to public
using (bucket_id = 'menu-photos');

drop policy if exists "menu_photos_staff_insert" on storage.objects;
create policy "menu_photos_staff_insert"
on storage.objects
for insert
to authenticated
with check (bucket_id = 'menu-photos' and public.is_staff());

drop policy if exists "menu_photos_staff_update" on storage.objects;
create policy "menu_photos_staff_update"
on storage.objects
for update
to authenticated
using (bucket_id = 'menu-photos' and public.is_staff())
with check (bucket_id = 'menu-photos' and public.is_staff());

drop policy if exists "menu_photos_staff_delete" on storage.objects;
create policy "menu_photos_staff_delete"
on storage.objects
for delete
to authenticated
using (bucket_id = 'menu-photos' and public.is_staff());
