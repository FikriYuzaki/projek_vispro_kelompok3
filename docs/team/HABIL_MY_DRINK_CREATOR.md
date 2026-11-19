# Habil — My Drink Creator

**Owner:** Habil  
**FeatureRegistry ID:** `drink-creator`  
**Branch:** `feature/habil-drink-creator`  
**Status di main:** ComingSoon sampai Fikri mengintegrasikan modul.

## 1. Tujuan fitur

Buat halaman untuk memilih minuman café dan mengatur komposisinya. Pelanggan dapat memilih opsi yang memang tersedia untuk item tersebut dan melihat harga akhir secara langsung sebelum item diteruskan ke alur pemesanan.

Fitur ini bukan editor menu staf dan bukan sistem checkout/payment.

## 2. Fitur wajib

1. **Pilih minuman:** tampilkan item dari `menu_items` yang tersedia (`is_available = true`). Tunjukkan nama, foto bila tersedia, kategori, harga dasar, dan apakah kustomisasi didukung.
2. **Opsi kustomisasi:** ambil pilihan dari `menu_options` berdasarkan `menu_item_id` dan `is_available = true`.
3. **Kelompok opsi:** dukung nilai `option_group` yang sah: `size`, `sugar`, `ice`, `milk`, `topping`, `espresso`.
4. **Harga dinamis:** total harga per unit = `menu_items.base_price` + jumlah `menu_options.extra_price` dari opsi yang dipilih. Perubahan opsi harus langsung memperbarui ringkasan.
5. **Ringkasan pilihan:** tunjukkan item, ukuran/opsi terpilih, harga dasar, tambahan, total per unit, kuantitas, dan subtotal.
6. **Validasi:** tolak item sold-out, opsi tidak tersedia, kuantitas <= 0, dan pilihan yang tidak lagi ada di database. Bila `supports_customization = false`, jangan mengarang opsi.
7. **Handoff:** sediakan hasil pilihan sebagai draft yang bisa diserahkan ke Friends Order Room / alur order. Jangan membuat checkout atau pembayaran sendiri.

## 3. Bentuk data Supabase yang harus dipakai

### `menu_items` (sudah ada)
- `id`, `name`, `category`, `description`, `base_price`, `image_url`, `is_available`, `supports_customization`.
- Kategori yang valid: `Coffee`, `Non-Coffee`, `Pastry`, `Dessert`.

Gunakan `MenuItem` dan `MenuService` yang sudah ada. Jangan membuat duplikat `MenuItem` atau mengedit `MenuService` milik Fikri.

### `menu_options` (sudah ada)
- `id`, `menu_item_id`, `option_group`, `option_name`, `extra_price`, `is_default`, `is_available`, `created_at`.
- Read tersedia untuk anon/authenticated; write hanya staff melalui kebijakan database.
- Jangan menambahkan opsi contoh statis bila data Supabase kosong. Empty state harus jujur.

### Snapshot untuk order
Ketika pilihan diserahkan ke order, bentuknya harus bisa dipetakan ke `order_items`:
- `menu_item_id`
- `item_name_snapshot`
- `unit_base_price`
- `options_snapshot` (JSON yang menyimpan nama group, option id/name, dan extra price)
- `options_extra_price`
- `quantity`
- `line_total`
- `notes`

Rumus: `line_total = (unit_base_price + options_extra_price) * quantity`.

## 4. File milik Habil

Buat file baru (nama boleh disesuaikan asalkan tetap dalam scope):
- `CuppaClub/Features/DrinkCreatorControl.vb`
- `CuppaClub/Features/DrinkCreatorControl.Designer.vb`
- `CuppaClub/Services/DrinkOptionService.vb`
- `CuppaClub/Models/MenuOption.vb`
- `CuppaClub/Models/ConfiguredDrinkDraft.vb` (draft pilihan, bukan model tabel Supabase)

Boleh tambah test/helper khusus fitur di lokasi yang sesuai.

**Jangan edit:** `FeatureRegistry.vb`, `FrmMain*`, `AppTheme.vb`, `SupabaseClientProvider.vb`, `MenuService.vb`, `CuppaClub.vbproj`, `schema.sql`, serta file Nisa.

## 5. Saran UX

- Layout dua area: daftar/menu pilihan di satu sisi dan panel builder/ringkasan di sisi lain.
- Pilihan ukuran dapat berupa radio/card; sugar/ice/milk bisa berupa radio atau selector sesuai datanya; topping/espresso dapat berupa checkbox jika beberapa opsi boleh dipilih.
- Jangan mengasumsikan setiap kelompok opsi harus selalu ada. Render berdasarkan opsi yang benar-benar tersedia.
- Gunakan harga Rupiah dan helper theme yang sudah ada.
- Sediakan state Loading, Error + Retry, Empty Menu, Empty Options, dan Ready.
- Jika minuman tidak mendukung customization, tetap izinkan memilih minuman dengan opsi default kosong dan harga dasar saja.

## 6. Handoff dengan Nisa

Habil tidak boleh bergantung pada kode branch Nisa yang belum tersedia. Keluarkan satu objek draft publik yang minimal memuat:
- MenuItemId
- ItemNameSnapshot
- UnitBasePrice
- OptionsSnapshot (data terstruktur yang bisa diserialisasi JSON)
- OptionsExtraPrice
- Quantity
- Notes
- UnitTotal dan LineTotal yang dihitung dari field di atas

Dokumentasikan cara membaca draft tersebut di PR. Nisa dapat membuat adapter pemetaan ke model room-order miliknya; Fikri akan menyatukan kontrak final saat integrasi. Jangan membuat record `orders` atau `order_items` secara langsung dari Drink Creator.

## 7. Acceptance criteria

- Bisa memilih item menu aktif dari Supabase.
- Opsi berasal dari `menu_options`, terfilter per item dan `is_available`.
- Setiap jenis option_group didukung tanpa mengunci UI pada daftar contoh.
- Perubahan opsi memperbarui harga dengan benar.
- Tidak ada harga negatif atau kuantitas nol/negatif.
- Ringkasan pilihan lengkap dan dapat diserahkan sebagai draft.
- Loading/error/empty states jelas.
- Designer tidak memanggil Supabase saat preview.
- Debug/Release build lulus.
- Tidak mengubah file milik anggota lain atau project-wide configuration.
