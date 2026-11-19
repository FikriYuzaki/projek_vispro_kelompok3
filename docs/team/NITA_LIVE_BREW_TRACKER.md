# Nita — Live Brew Tracker

**Owner:** Nita  
**FeatureRegistry ID:** `brew-tracker`  
**Branch:** `feature/nita-brew-tracker`  
**Status di main:** ComingSoon sampai Fikri mengintegrasikan modul.

## 1. Tujuan fitur

Buat halaman pemantauan pesanan café. Pelanggan melihat progres pesanan miliknya; staf melihat antrian kerja dan memperbarui status pesanan secara berurutan. Setiap perubahan status harus tercatat pada history database.

## 2. Fitur wajib

1. **Customer order tracking:** pelanggan melihat hanya pesanan miliknya dan pesanan yang memang menjadi bagian room yang dia ikuti.
2. **Staff queue:** staf/admin melihat antrian pesanan dan detail item yang perlu dibuat.
3. **Status progress:** tampilkan tahapan `Waiting` → `Preparing` → `Ready` → `Completed`.
4. **Staff status update:** hanya staff/admin yang dapat mengubah status, dan hanya satu tahap berikutnya yang boleh dipilih.
5. **History:** tampilkan riwayat perubahan status dengan waktu dan pelaku dari `order_status_history`.
6. **Refresh/live update:** gunakan Realtime dengan lifecycle yang benar jika diimplementasikan; sediakan refresh fallback dan indikator waktu update terakhir.

## 3. Schema yang harus digunakan

### `orders`
- `id`, `order_number`, `customer_id`, `room_id`, `status`, `payment_method`, `payment_status`, `subtotal`, `discount_amount`, `total_amount`, `created_at`, `updated_at`.
- Status persis: `Waiting`, `Preparing`, `Ready`, `Completed`.
- RLS mengizinkan customer membaca order miliknya, anggota room membaca order room mereka, dan staff membaca semua order.
- Update `orders` hanya untuk staff/admin melalui RLS.

### `order_items`
- `order_id`, `menu_item_id`, `member_user_id`, `item_name_snapshot`, `unit_base_price`, `options_snapshot`, `options_extra_price`, `quantity`, `line_total`, `notes`, `created_at`.
- Ambil detail item untuk order yang memang boleh dibaca user.
- Jangan mengubah snapshot atau membuat history palsu.

### `order_status_history`
- `order_id`, `previous_status`, `new_status`, `changed_by`, `changed_at`.
- SELECT mengikuti izin order terkait.
- Trigger `trg_orders_status_transition` otomatis menulis history saat order dibuat atau status berubah.
- Jangan INSERT history secara manual dari aplikasi.

### Aturan transisi database

Trigger hanya menerima:
- `Waiting` → `Preparing`
- `Preparing` → `Ready`
- `Ready` → `Completed`

Transisi melompat atau mundur akan ditolak. UI harus menampilkan hanya aksi status berikutnya untuk staff. Jangan mencoba bypass trigger.

## 4. File milik Nita

Buat file baru:
- `CuppaClub/Features/BrewTrackerControl.vb`
- `CuppaClub/Features/BrewTrackerControl.Designer.vb`
- `CuppaClub/Services/BrewTrackerService.vb`
- model Supabase khusus (`Order`, `OrderItem`, `OrderStatusHistory`) di `CuppaClub/Models/` jika belum ada.

Jangan mengubah model `MenuItem`, `UserProfile`, `MenuService`, atau layanan shared milik Fikri. Sebelum membuat model, pastikan tidak ada kelas dengan nama/tujuan yang sama di branch lokal.

**Jangan edit:** `FeatureRegistry.vb`, `FrmMain*`, `AppTheme.vb`, `SupabaseClientProvider.vb`, `AppSession.vb`, `CuppaClub.vbproj`, schema baseline, atau file Nisa.

## 5. Saran UX

- Staff view: daftar/queue dengan nomor order, waktu, status, ringkasan item, dan kontrol status yang jelas.
- Customer view: progress timeline sederhana, nomor order, item, dan waktu update.
- Pisahkan tampilan staff dan customer berdasarkan `AppSession.IsStaff`.
- Customer tidak boleh melihat semua antrean.
- Staff tidak boleh mengubah status menjadi `Completed` langsung dari `Waiting`.
- Tunjukkan status dengan label + warna, bukan warna saja.
- Loading/error/empty/unauthorized states harus jelas.

## 6. Realtime dan performa

`SupabaseClientProvider` saat ini mengatur `AutoConnectRealtime = False`. Jika memakai Supabase Realtime:
- subscribe secara eksplisit ke perubahan `orders` yang relevan;
- handle channel state, error, reconnect, dan unsubscribe saat control dispose;
- jangan membuat satu subscription baru setiap kali tombol refresh ditekan;
- sediakan refresh manual bila Realtime tidak siap.

Hindari polling yang terlalu sering. Bila memilih polling, gunakan interval moderat dan stop timer saat control dispose atau tidak aktif.

## 7. Acceptance criteria

- Customer hanya melihat order sendiri / room yang diikuti.
- Staff dapat melihat antrean sesuai RLS.
- Detail item tampil dari `order_items`.
- Update status hanya staff dan hanya transisi valid berikutnya.
- History otomatis tampil dari `order_status_history`.
- Invalid transition menghasilkan pesan yang wajar, bukan crash.
- Subscription/timer dibersihkan saat dispose dan tidak duplikat.
- Designer tidak memanggil Supabase saat preview.
- Debug/Release build lulus.
- Tidak ada perubahan skema/akses database tanpa migration review.
