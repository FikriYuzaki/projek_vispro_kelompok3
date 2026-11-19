# Nisa — Friends Order Room

**Owner:** Nisa  
**FeatureRegistry ID:** `friends-order`  
**Branch:** `feature/nisa-friends-order`  
**Status di main:** ComingSoon sampai Fikri mengintegrasikan modul.

## 1. Tujuan fitur

Buat ruang pemesanan bersama sehingga satu pelanggan dapat membuat room, membagikan kode undangan, dan teman-temannya bergabung menggunakan kode tersebut. Anggota dapat melihat pesanan grup, menambahkan item atas nama masing-masing, melihat subtotal, dan host dapat mengunci room sebelum pesanan dikirim.

## 2. Fitur wajib

1. **Create room:** pelanggan yang login membuat room dengan judul dan kode unik 4–16 karakter. Host menjadi anggota otomatis melalui trigger database.
2. **Join via code:** pengguna yang login memasukkan kode. Aplikasi harus memanggil RPC `join_room_by_code(p_room_code text)`; jangan melakukan SELECT semua room untuk mencari kode.
3. **Room overview:** setelah menjadi anggota, tampilkan judul, status, host/anggota, dan siapa yang tergabung.
4. **Group order draft:** tampilkan daftar item per anggota dan subtotal per anggota serta total grup. Handoff item harus kompatibel dengan `order_items` (lihat Shared Rules).
5. **Lock room:** host dapat mengunci room sebelum order dikirim. Room yang statusnya bukan `open` tidak menerima anggota baru.
6. **Status dan error:** kode salah, room locked/cancelled/submitted, belum login, akses ditolak, jaringan gagal, dan room kosong harus punya pesan berbeda.

## 3. Schema dan aturan keamanan yang sudah ada

### `order_rooms`
- `id`, `room_code`, `title`, `host_user_id`, `status`, `created_at`, `locked_at`, `submitted_at`.
- Status yang sah: `open`, `locked`, `submitted`, `cancelled`.
- Create room diizinkan untuk authenticated dengan `host_user_id = auth.uid()`.
- SELECT dibatasi untuk host, room member, atau staff. Jangan membuat query daftar room terbuka.
- Update hanya host atau staff menurut RLS.

### `room_members`
- `id`, `room_id`, `user_id`, `display_name`, `joined_at`.
- SELECT hanya member/staff.
- Direct INSERT/UPDATE/DELETE dicabut dari anon dan authenticated.
- Host otomatis ditambahkan sebagai member oleh trigger `trg_order_rooms_add_host_member`.
- Member baru harus masuk melalui RPC `join_room_by_code`.

### RPC

`public.join_room_by_code(p_room_code text)`:
- hanya authenticated;
- normalisasi input dilakukan server-side;
- menerima room yang statusnya `open` saja;
- menolak kode salah/tidak tersedia/locked/submitted/cancelled;
- membuat membership dan mengembalikan data minimal (`room_id`, `title`, `status`, `joined_at`).

Jangan mengganti RPC dengan direct insert ke `room_members`. Jangan menampilkan daftar kode undangan.

**Penting:** migration `20261119_secure_order_rooms_rls_and_rpc.sql` sudah ada di repository, tetapi pengujian PostgreSQL lokal tidak membuktikan bahwa migration sudah diterapkan ke Supabase proyek bersama. Jangan jalankan migration di Supabase bersama sendiri. Bila RPC belum ada/ditolak, tampilkan instruksi yang aman dan informasikan Fikri untuk menerapkan migration setelah review.

### `orders` dan `order_items`

- `orders` mempunyai `customer_id`, `room_id`, `status`, `payment_status`, `subtotal`, `discount_amount`, `total_amount`.
- `order_items` mempunyai `order_id`, `menu_item_id`, `member_user_id`, snapshot nama/harga/opsi, quantity, line_total, notes.
- Untuk model room, gunakan satu order induk yang terkait room dan line item yang menandai `member_user_id`, kecuali Fikri menyetujui model berbeda.
- Jangan menambahkan alur pembayaran riil. Gunakan status simulasi yang sudah tersedia.
- Jangan mengarang policy/privilege baru. Periksa RLS sebelum mengasumsikan suatu operasi bisa dilakukan.

## 4. File milik Nisa

Buat file baru:
- `CuppaClub/Features/FriendsOrderControl.vb`
- `CuppaClub/Features/FriendsOrderControl.Designer.vb`
- `CuppaClub/Services/FriendsOrderService.vb`
- `CuppaClub/Models/OrderRoom.vb`
- `CuppaClub/Models/RoomMember.vb`
- model draft order room khusus bila diperlukan.

Migration khusus fitur boleh dibuat dengan nama timestamp baru, tetapi perubahan SQL wajib direview Fikri sebelum diterapkan. Jangan mengedit migration 19 November yang sudah merge.

**Jangan edit:** `FeatureRegistry.vb`, `FrmMain*`, shared `SupabaseClientProvider`, `AuthService`, `AppSession`, `AppTheme`, `CuppaClub.vbproj`, baseline `schema.sql`, atau file Habil.

## 5. Aturan interaksi

- Create/join/lock harus mensyaratkan login.
- Hanya host yang boleh mengunci room; sembunyikan/disable action untuk member biasa, tetapi tetap andalkan RLS di database.
- Setelah lock, tombol join dan penambahan item baru harus berhenti sesuai kebijakan produk. Jangan mengubah status menjadi `submitted` tanpa alur order yang disetujui.
- Jangan menganggap Realtime tersambung otomatis. `SupabaseClientProvider` memiliki `AutoConnectRealtime = False`; bila menggunakan channel, subscribe eksplisit, tampilkan state koneksi, dan unsubscribe ketika kontrol dispose. Sediakan Refresh manual.
- Jangan membuat room code dengan pola mudah ditebak berurutan. Gunakan generator acak dengan retry saat unique constraint collision.

## 6. Handoff dengan Habil dan Nita

- Habil menyediakan draft minuman terkonfigurasi. Buat adapter agar field-nya menjadi snapshot `order_items` (`menu_item_id`, `item_name_snapshot`, `unit_base_price`, `options_snapshot`, `options_extra_price`, `quantity`, `line_total`, `notes`).
- Nita membaca order dan item yang dihasilkan dari room ini; jangan menulis status produksi dari Friends Order Room.
- Dokumentasikan asumsi model order grup dan edge cases dalam PR agar Fikri bisa mengintegrasikan tanpa mengubah data existing.

## 7. Acceptance criteria

- Host dapat membuat room; host tercatat sebagai member melalui trigger.
- Pengguna valid dapat bergabung dengan kode melalui RPC.
- Kode salah/room non-open/unauthenticated ditolak.
- Tidak ada query yang mengekspos kode room terbuka ke user lain.
- Direct insert ke `room_members` tidak digunakan.
- Host dapat lock room; non-host tidak dapat melakukannya.
- Daftar anggota dan subtotal hanya terlihat sesuai RLS.
- Realtime lifecycle/refresh ditangani dengan aman.
- Designer aman dari koneksi database saat preview.
- Debug/Release build lulus.
- Semua operasi dan migration dijelaskan dalam PR; tidak ada migration produksi yang dijalankan sendiri.
