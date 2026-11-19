# CuppaClub — Shared Team Rules

## 1. Konteks proyek

CuppaClub adalah aplikasi desktop café companion dan counter app bertema sweet/cozy café. Aplikasi dibuat untuk pelanggan dan staf café, dengan pengalaman desktop yang rapi dan fitur online bersama melalui Supabase.

**Stack yang sudah ditetapkan:**
- VB.NET, Windows Forms, .NET 10 (`net10.0-windows`).
- Visual Studio 2026.
- Supabase (PostgreSQL, Auth, Realtime, Storage).
- NuGet `Supabase` versi `8.12.0`.
- WinForms Hybrid: kerangka UI tetap didefinisikan pada pasangan `*.Designer.vb` dan code-behind `*.vb`; data dan bagian UI yang dinamis tetap dibuat saat runtime.

Repository: `https://github.com/FikriYuzaki/projek_vispro_kelompok3`

## 2. Status fondasi yang sudah tersedia

PR #1 sudah di-Squash Merge ke `main` pada 19 November 2026. Fondasi yang sudah ada:

- `FrmMain` sebagai shell aplikasi.
- `FeatureRegistry` berisi lima ID fitur resmi dan status awal.
- `ComingSoonControl` sebagai placeholder untuk fitur yang belum diintegrasikan.
- `MenuGalleryControl` + `MenuService` sebagai Feature 01 yang sudah aktif.
- Login dan session: `LoginForm`, `AuthService`, `AppSession`, `UserProfile`.
- Konfigurasi Supabase melalui `SupabaseClientProvider` dan `AppConfig`.
- `AppTheme` dan `DESIGN.md` sebagai sumber visual utama.
- Schema Supabase baseline di `supabase/schema.sql` dan migration di `supabase/migrations/`.

**Jangan membuat ulang fondasi ini.** Baca file yang sudah ada sebelum membuat class/service pengganti.

## 3. Fitur, pemilik, dan ID resmi

| ID FeatureRegistry | Nama | Pemilik | Status awal |
|---|---|---|---|
| `menu-gallery` | Café Menu Gallery | Fikri | Available |
| `drink-creator` | My Drink Creator | Habil | ComingSoon |
| `friends-order` | Friends Order Room | Nisa | ComingSoon |
| `brew-tracker` | Live Brew Tracker | Nita | ComingSoon |
| `sweet-rewards` | Sweet Rewards Club | Visel | ComingSoon |

Jangan mengganti ID, nama resmi, pemilik, atau status fitur dari branch anggota. Fikri akan mengaktifkan view melalui `FeatureRegistry` setelah modul direview dan siap diintegrasikan.

## 4. Aturan UI dan Visual Designer

Sebelum mengubah UI, baca `DESIGN.md` dan `AGENTS.md`. Ikuti instruksi anti-slop di `AGENTS.md` dan skill lokal yang dirujuk di sana.

Arah desain:
- Canvas vanilla cream `#FAF4EC`, surface warm milk `#FFFDF9`, sidebar soft oat `#F3E8DC`.
- Dark espresso `#3B261C`, muted roast `#6E5647`, mocha action `#6B442E`, strawberry accent `#B85467`, sage success `#EAF2EC`.
- Font `Segoe UI`.
- Default shell `1200x760`; minimum `1024x680`.
- Spacing utama `8/12/16/24px`, border tipis almond/roast.
- Motion tenang: tidak ada animasi dekoratif terus-menerus, bouncing, atau efek ramai.
- Jangan membuat fake stats, rating, testimonial, atau tombol yang tidak bekerja.
- Semua status harus memiliki teks, jangan mengandalkan warna saja.

Setiap fitur harus dibuat sebagai UserControl mandiri dengan pasangan:
- `CuppaClub/Features/<FeatureName>Control.vb`
- `CuppaClub/Features/<FeatureName>Control.Designer.vb`

Pola constructor:
1. Panggil `InitializeComponent()`.
2. Deteksi design-time dengan pola yang sudah digunakan oleh kontrol di `main` (`LicenseManager.UsageMode = LicenseUsageMode.Designtime OrElse DesignMode`).
3. Jangan membuat koneksi Supabase, memuat session, menjalankan query, atau memulai timer/subscription saat Designer membuka kontrol.
4. Pasang event handler dan inisialisasi runtime hanya setelah lolos guard design-time.
5. Bersihkan timer, subscription, event handler, gambar, dan resource di `Dispose`.

Kontrol Designer harus punya constructor tanpa parameter (`Public Sub New()`). Jangan menghapus file Designer, menggandakan kontrol, atau memindahkan kode runtime ke `InitializeComponent()`.

## 5. Batas file dan kepemilikan

Anggota boleh menambah file baru di folder fitur masing-masing dan membuat service/model khusus fiturnya. Jangan mengedit file pusat berikut tanpa koordinasi dengan Fikri:

- `CuppaClub/Forms/FrmMain.vb` dan `FrmMain.Designer.vb`
- `CuppaClub/Core/FeatureRegistry.vb`
- `CuppaClub/Core/AppTheme.vb`
- `CuppaClub/Core/AppConfig.vb`, `AppSession.vb`
- `CuppaClub/Services/SupabaseClientProvider.vb`, `AuthService.vb`, `MenuService.vb`
- `CuppaClub/CuppaClub.vbproj`
- `supabase/schema.sql` (baseline)
- Migration milik anggota lain

SDK-style project sudah memasukkan file `.vb` secara default. Untuk menghindari konflik di `.vbproj`, jangan menambahkan metadata `<SubType>` / `<DependentUpon>` sendiri. Buat pasangan file `.vb` dan `.Designer.vb` dengan benar; Fikri akan menambahkan metadata nesting untuk seluruh UserControl baru dalam satu perubahan integrasi.

Bila perlu perubahan database:
- Buat migration baru dengan nama timestamp unik di `supabase/migrations/`.
- Jangan mengedit migration yang sudah merge.
- Jangan mengubah baseline schema kecuali Fikri meminta.
- Jangan menjalankan migration ke Supabase bersama/produksi.
- Sertakan alasan, SQL, rollback/risiko, dan tes dalam PR agar direview terlebih dahulu.

## 6. Aturan Supabase, Auth, dan keamanan

- Gunakan `SupabaseClientProvider.GetClientAsync()` untuk memperoleh client bersama. Jangan membuat Supabase client baru dengan key yang ditulis di source.
- Gunakan `AppSession.CurrentUser`, `AppSession.IsAuthenticated`, dan `AppSession.IsStaff` untuk menyesuaikan UI; otorisasi sebenarnya tetap harus dijamin RLS/RPC di database.
- Role yang sah: `customer`, `staff`, `admin`. Pendaftaran baru selalu menjadi `customer`; jangan membuat fitur untuk self-promote role.
- Jangan meminta, menampilkan, menyimpan di source, atau meng-commit `service_role`, `sb_secret_`, atau credential pribadi. Desktop client hanya memakai publishable/anon key.
- Jangan menganggap menyembunyikan tombol sebagai keamanan. RLS tetap harus menolak aksi yang tidak berhak.
- `SupabaseClientProvider` saat ini mengatur `AutoConnectRealtime = False`. Jika memakai Realtime, buat subscription secara eksplisit, tangani error, dan lepaskan subscription saat kontrol dispose. Sediakan refresh/manual fallback yang jujur.
- Konfigurasi lokal `appsettings.local.json` diabaikan oleh `.gitignore`. Jangan commit file itu.

### Status migration Friends Order Room

Main memuat `supabase/migrations/20261119_secure_order_rooms_rls_and_rpc.sql` dan baseline schema dengan `join_room_by_code`. Pengujian PostgreSQL 18 yang dilaporkan adalah pengujian lokal terisolasi. Itu **tidak membuktikan migration sudah diterapkan ke proyek Supabase bersama**. Nisa harus menangani kegagalan RPC/migration dengan pesan yang jelas dan mengabari Fikri; jangan mengubah database bersama sendiri.

## 7. Data contract lintas fitur

Semua anggota harus menjaga bentuk data yang disimpan di `order_items`:

| Kolom | Makna |
|---|---|
| `order_id` | Order induk |
| `menu_item_id` | Item menu dari `menu_items` |
| `member_user_id` | Anggota room/pelanggan yang memilih item (nullable untuk order non-room) |
| `item_name_snapshot` | Nama item ketika dipesan |
| `unit_base_price` | Harga dasar saat dipilih |
| `options_snapshot` | JSON snapshot opsi yang dipilih |
| `options_extra_price` | Total tambahan harga opsi per unit |
| `quantity` | Kuantitas, wajib lebih dari nol |
| `line_total` | Total baris sesuai harga dan kuantitas |
| `notes` | Catatan pelanggan |

Kontrak harga: `unit_total = unit_base_price + options_extra_price`; `line_total = unit_total * quantity`. Simpan snapshot nama/harga/opsi agar perubahan menu setelah order tidak mengubah riwayat order. Jangan mengandalkan harga yang dikirim UI sebagai satu-satunya validasi untuk transaksi sungguhan; usulkan validasi server-side bila dibutuhkan.

## 8. Error/loading/empty states dan UX

Setiap tampilan remote-data harus mengatasi:
- Unconfigured (Supabase belum dikonfigurasi).
- Loading (ada indikator yang tidak mengunci seluruh aplikasi tanpa alasan).
- Empty database (belum ada data).
- Empty filter/search (data ada, tetapi tidak cocok dengan filter).
- Error (pesan manusiawi dan tombol Retry bila aman).
- Success/ready.
- Unauthorized/forbidden (jelaskan bahwa peran/session tidak punya izin, jangan menyebutnya network error).

Jangan menyamarkan error sebagai data kosong. Jangan menampilkan stack trace atau credential ke pengguna.

## 9. Git workflow

Branch masing-masing:
- `feature/habil-drink-creator`
- `feature/nisa-friends-order`
- `feature/nita-brew-tracker`
- `feature/visel-sweet-rewards`

Mulai dari `origin/main` terbaru:

```bash
git fetch origin
git switch main
git pull --ff-only origin main
git switch -c feature/<nama-branch>
```

Selama kerja:

```bash
git status
git diff --check
dotnet build CuppaClub.slnx --configuration Debug
dotnet build CuppaClub.slnx --configuration Release
```

Commit kecil dan jelas. Contoh:
- `feat(drink-creator): add configurable drink builder`
- `feat(friends-order): add room create and join flow`
- `feat(brew-tracker): add staff order status board`
- `feat(sweet-rewards): add points and rewards view`

Push branch sendiri, lalu buat PR ke `main`. Jangan push ke branch anggota lain, jangan commit langsung ke `main`, jangan force-push, dan jangan merge PR sendiri. Jangan memakai `git add .` tanpa memeriksa staged files. Pastikan `.vs/`, `bin/`, `obj/`, `.env`, `appsettings.local.json`, dan secret tidak ikut.

## 10. Acceptance gate sebelum PR

- Build Debug dan Release sukses.
- UserControl bisa dibuat dengan constructor tanpa parameter.
- Tidak ada koneksi/query saat dibuka di Designer.
- Designer dan runtime sama-sama bisa dibuka.
- UI konsisten dengan `DESIGN.md`, nyaman pada 1200x760 dan minimum 1024x680.
- Semua tombol punya aksi nyata atau tampil disabled dengan alasan jelas.
- Hak akses diuji sebagai customer dan staff sesuai peran.
- Tidak ada perubahan pusat di luar scope tanpa persetujuan.
- PR menyebut file, alur fitur, migration, cara uji, hasil uji, dan keterbatasan yang masih ada.
