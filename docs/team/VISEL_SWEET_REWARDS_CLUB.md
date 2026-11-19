# Visel — Sweet Rewards Club

**Owner:** Visel  
**FeatureRegistry ID:** `sweet-rewards`  
**Branch:** `feature/visel-sweet-rewards`  
**Status di main:** ComingSoon sampai Fikri mengintegrasikan modul.

## 1. Tujuan fitur

Buat halaman loyalitas pelanggan yang menampilkan saldo poin, katalog reward, detail syarat penukaran, dan riwayat transaksi poin. Pelanggan dapat memahami reward yang tersedia dan proses penukaran tanpa saldo atau stok menjadi negatif.

## 2. Fitur wajib

1. **Points balance:** tampilkan saldo dari ledger `reward_transactions`, bukan dari angka buatan atau hardcoded.
2. **Reward catalog:** tampilkan `reward_catalog` yang `is_available = true`, dengan biaya poin dan stock bila nilainya tersedia.
3. **Redeem flow:** pelanggan memilih reward dan melihat konfirmasi biaya poin. Penukaran harus divalidasi dan diproses atomik di server/database.
4. **History:** tampilkan transaksi poin masuk/keluar milik user, termasuk tipe, tanggal, deskripsi, dan delta.
5. **Empty/error/auth states:** belum login, belum ada poin, katalog kosong, reward habis, saldo kurang, request gagal, dan penukaran sukses harus dibedakan.

## 3. Schema yang sudah ada

### `reward_catalog`
- `id`, `title`, `description`, `points_cost`, `is_available`, `stock_qty`, `created_at`, `updated_at`.
- Read publik tersedia; write catalog hanya staff melalui RLS.

### `reward_transactions`
- `id`, `user_id`, `order_id`, `reward_id`, `tx_type`, `points_delta`, `description`, `created_at`.
- `tx_type` valid: `earn`, `redeem`, `adjust`.
- `points_delta` tidak boleh nol; earn biasanya positif, redeem negatif.
- Read hanya untuk pemilik transaksi atau staff.
- Ada unique partial index `uq_reward_earn_per_order` agar satu order tidak mendapat earn lebih dari sekali.

Saldo dapat dihitung sebagai `SUM(points_delta)` untuk user yang login. Jangan menambah saldo palsu di sisi UI dan jangan mengubah ledger lama.

## 4. Batas keamanan yang sangat penting

Pada schema baseline saat ini, client authenticated diberi hak `SELECT` pada `reward_transactions`, tetapi tidak diberi hak INSERT/UPDATE/DELETE. Ini disengaja agar pelanggan tidak bisa menambah poin atau mengurangi harga reward sendiri.

**Belum ada RPC earn/redeem khusus rewards yang disediakan pada baseline.** Oleh karena itu:
- Jangan mencoba `INSERT` langsung ke `reward_transactions` dari desktop client.
- Jangan memberi GRANT write luas atau mengubah RLS agar pelanggan dapat menulis bebas.
- Jangan melakukan pengurangan stok dan penulisan ledger sebagai dua request client terpisah; itu rawan race condition.
- Usulkan migration berisi RPC `redeem_reward` yang memvalidasi `auth.uid()`, reward tersedia, stok, dan saldo; lakukan decrement stok + insert ledger dalam satu transaksi server-side.
- Untuk earn, usulkan mekanisme server-side yang memverifikasi order eligible dan mematuhi unique index per order. Formula konversi transaksi menjadi poin belum ditetapkan dalam schema; tulis usulan formula untuk disetujui Fikri sebelum hardcode.
- Siapkan tes PostgreSQL lokal untuk saldo tidak cukup, stok habis, request ganda, user mencoba menukar reward milik user lain, dan atomic rollback.
- Migration tidak boleh diterapkan ke Supabase bersama sampai Fikri mereview dan memberi persetujuan.

Selama RPC belum disetujui/tersedia di environment, tampilkan UI katalog dan riwayat dengan tombol redeem disabled serta keterangan yang jujur seperti "Penukaran belum tersedia". Jangan berpura-pura penukaran berhasil.

## 5. File milik Visel

Buat file baru:
- `CuppaClub/Features/SweetRewardsControl.vb`
- `CuppaClub/Features/SweetRewardsControl.Designer.vb`
- `CuppaClub/Services/RewardsService.vb`
- `CuppaClub/Models/RewardCatalogItem.vb`
- `CuppaClub/Models/RewardTransaction.vb`

Migration baru boleh dibuat di `supabase/migrations/` setelah kontrak RPC dijelaskan. Jangan mengubah baseline `schema.sql`, migration yang sudah merge, `SupabaseClientProvider`, `AuthService`, `AppSession`, atau file anggota lain.

**Jangan edit:** `FeatureRegistry.vb`, `FrmMain*`, `AppTheme.vb`, `CuppaClub.vbproj`, model `UserProfile` / `MenuItem`, atau schema baseline.

## 6. Saran UX

- Header saldo poin yang jelas dan dapat ditelusuri ke ledger.
- Katalog reward yang menonjolkan judul, deskripsi, biaya poin, dan stok/tersedia.
- Tampilkan perbedaan poin masuk dan poin keluar melalui label teks seperti `+120 poin` atau `-300 poin`.
- Berikan konfirmasi sebelum redeem.
- Jangan tampilkan grafik/statistik palsu atau reward contoh yang tidak berasal dari database.
- Pastikan pelanggan hanya melihat saldo dan transaksi miliknya.

## 7. Acceptance criteria

- Saldo berasal dari penjumlahan ledger Supabase untuk user login.
- Katalog berasal dari `reward_catalog`.
- Riwayat hanya berisi transaksi user tersebut (kecuali staff view yang memang disetujui).
- Tidak ada direct write dari client ke `reward_transactions`.
- RPC/migration redeem dirancang atomik, diuji lokal, dan direview sebelum digunakan.
- Formula earn ditulis eksplisit dan menunggu persetujuan owner sebelum diimplementasikan.
- State saldo kurang, stok habis, belum login, kosong, error, dan sukses jelas.
- Designer aman tanpa koneksi Supabase.
- Debug/Release build lulus.
