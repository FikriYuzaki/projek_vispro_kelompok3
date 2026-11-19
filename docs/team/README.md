# CuppaClub — Team Feature Handoff

Dokumen ini adalah paket instruksi kerja untuk empat anggota tim yang mengembangkan fitur CuppaClub secara paralel.

## Siapa membaca apa?

| Anggota | Dokumen wajib | Branch |
|---|---|---|
| Semua anggota | `SHARED_RULES.md` | — |
| Habil | `HABIL_MY_DRINK_CREATOR.md` | `feature/habil-drink-creator` |
| Nisa | `NISA_FRIENDS_ORDER_ROOM.md` | `feature/nisa-friends-order` |
| Nita | `NITA_LIVE_BREW_TRACKER.md` | `feature/nita-brew-tracker` |
| Visel | `VISEL_SWEET_REWARDS_CLUB.md` | `feature/visel-sweet-rewards` |

## Urutan mulai

1. Pastikan PR #1 sudah masuk ke `main` (sudah di-merge pada 19 November 2026).
2. Setiap anggota membaca `SHARED_RULES.md` dan dokumen fiturnya sendiri.
3. Setiap anggota membuat branch tugas dari `origin/main` terbaru. Jangan membuat branch dari branch anggota lain.
4. Kerjakan hanya batas tanggung jawab yang tertulis pada dokumen masing-masing.
5. Push branch dan buka Pull Request ke `main`. Jangan merge sendiri.

## Struktur dokumen

- `SHARED_RULES.md`: konteks produk, stack, visual system, pembagian file, keamanan, integrasi, Git, dan acceptance gate bersama.
- Empat file anggota: tujuan, fitur yang wajib selesai, data Supabase yang relevan, file yang boleh dibuat, larangan, dan kriteria penerimaan masing-masing.

## Catatan status database

Schema baseline dan migration keamanan Friends Order Room tersedia di folder `supabase/`. Tes PostgreSQL lokal tidak sama dengan penerapan ke Supabase proyek yang dipakai bersama. Jangan menganggap migration sudah diterapkan ke proyek Supabase bersama kecuali pemilik proyek mengonfirmasi atau memverifikasinya di Dashboard.
