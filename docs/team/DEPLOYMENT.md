# Panduan Publish dan Distribusi CuppaClub (Windows)

Dokumen ini menjelaskan cara menyiapkan konfigurasi Supabase dan mem-publish CuppaClub sebagai aplikasi Windows self-contained agar dapat dijalankan langsung di komputer lain tanpa konfigurasi manual.

## 1. Mengambil URL dan Publishable Key Supabase

1. Buka Dashboard proyek Supabase CuppaClub di browser.
2. Masuk ke menu **Project Settings** lalu pilih bagian **API** (atau **API Keys** pada antarmuka terbaru).
3. Salin dua nilai berikut:
   - **Project URL** (`https://<project-ref>.supabase.co`)
   - **Publishable key** (`sb_publishable_...`) atau **anon public key** (JWT legacy yang masih kompatibel).

## 2. Menyiapkan `CuppaClub/appsettings.json` secara Lokal

File `CuppaClub/appsettings.json` diabaikan oleh `.gitignore` agar kredensial publish proyek tidak ikut ter-commit ke repository.

1. Salin `appsettings.example.json` di root repository menjadi `CuppaClub/appsettings.json`.
2. Isi nilai `SupabaseUrl` dan `SupabaseAnonKey` dengan kredensial proyek Supabase milik tim:

```json
{
  "SupabaseUrl": "https://YOUR_PROJECT_REF.supabase.co",
  "SupabaseAnonKey": "sb_publishable_YOUR_PUBLISHABLE_OR_ANON_KEY"
}
```

3. Simpan file dengan encoding UTF-8.

Catatan kompatibilitas: nama properti JSON tetap `SupabaseAnonKey` demi kompatibilitas mundur dan dapat menerima key modern berawalan `sb_publishable_...` maupun legacy `anon` JWT.

## 3. Urutan Prioritas Pembacaan Konfigurasi di Runtime

Saat aplikasi berjalan, `AppConfig.Load()` membaca konfigurasi Supabase dengan urutan prioritas berikut:

1. `%LocalAppData%\CuppaClub\supabase.json` (override lokal per user, misalnya melalui dialog **Pengaturan Supabase** di aplikasi).
2. `appsettings.local.json` (konfigurasi development lokal pada direktori kerja / repository).
3. Environment variables (`CUPPACLUB_SUPABASE_URL` / `SUPABASE_URL` dan `CUPPACLUB_SUPABASE_ANON_KEY` / `SUPABASE_ANON_KEY`).
4. `appsettings.json` di sebelah `CuppaClub.exe` (`AppContext.BaseDirectory`), sebagai konfigurasi bawaan hasil publish.

## 4. Cara Publish Aplikasi Windows (`win-x64`)

Target `ValidatePackagedSupabaseConfig` akan memeriksa `CuppaClub/appsettings.json` secara otomatis sebelum proses publish dimulai. Jika file tidak ada, masih berisi placeholder, bukan HTTPS, atau memakai secret key, proses publish akan dibatalkan.

`CuppaClub.vbproj` mendeklarasikan `<AppDesignerFolder>My Project</AppDesignerFolder>` secara eksplisit agar baik Visual Studio 2026 maupun `.NET 10 SDK` CLI sama-sama menemukan `CuppaClub/My Project/PublishProfiles/FolderProfile.pubxml` secara otomatis.

### Opsi A: Menggunakan .NET CLI (Memakai Profil Resmi)

Jalankan dari root repository:

```powershell
dotnet publish CuppaClub/CuppaClub.vbproj -p:PublishProfile=FolderProfile
```

Atau dengan path absolut ke file `.pubxml`:

```powershell
dotnet publish CuppaClub/CuppaClub.vbproj -p:PublishProfileFullPath="C:\path\ke\CuppaClub\My Project\PublishProfiles\FolderProfile.pubxml"
```

Alternatif tanpa file profil (parameter CLI langsung):

```powershell
dotnet publish CuppaClub/CuppaClub.vbproj -c Release -r win-x64 --self-contained true
```

Hasil publish akan berada di:

```text
CuppaClub\bin\Release\net10.0-windows\win-x64\publish\
```

### Opsi B: Menggunakan Visual Studio 2026 (Folder Profile)

1. Klik kanan proyek **CuppaClub** di Solution Explorer lalu pilih **Publish...**.
2. Pilih profil **FolderProfile** (`CuppaClub/My Project/PublishProfiles/FolderProfile.pubxml`).
3. Profil sudah dikonfigurasi dengan:
   - Configuration: `Release`
   - Target Framework: `net10.0-windows`
   - Deployment Mode: `Self-contained`
   - Target Runtime: `win-x64`
   - Produce single file: `false` (tidak dicentang, agar file sidecar `appsettings.json` tetap terbaca)
   - PublishDir / PublishUrl: `bin\Release\net10.0-windows\win-x64\publish\`
4. Klik **Publish**.

## 5. Cara Mendistribusikan Hasil Publish ke Komputer Lain

**Penting:** jangan menyalin `CuppaClub.exe` saja.

1. Kompres seluruh isi folder `CuppaClub\bin\Release\net10.0-windows\win-x64\publish\` menjadi satu file `.zip`.
2. Di komputer Windows tujuan, ekstrak seluruh isi file `.zip` tersebut ke satu folder yang sama.
3. Pastikan file `CuppaClub.exe`, `appsettings.json`, seluruh `.dll`, dan dependensi runtime berada di folder yang sama.
4. Jalankan `CuppaClub.exe`. Aplikasi akan langsung terhubung ke Supabase Cloud memakai `appsettings.json` bawaan tanpa perlu mengisi URL dan key secara manual.

## 6. Kredensial yang Dilarang Dibundel

Jangan pernah memasukkan kredensial berikut ke `CuppaClub/appsettings.json`, repository Git, atau paket distribusi EXE:

- Key berawalan `sb_secret_`
- Legacy JWT dengan role `service_role`
- Password database PostgreSQL
- Connection string PostgreSQL langsung (`postgres://...` / `Host=...;Password=...`)

Kredensial di atas mem-bypass Row Level Security (RLS) dan hanya boleh disimpan di server atau dashboard pribadi.

