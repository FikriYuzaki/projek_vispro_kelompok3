# DESIGN.md - CuppaClub Visual Direction

> Single source of visual truth for CuppaClub (VB.NET + Windows Forms desktop application).

## Design Read and Liveliness Dials

Reading this as: Windows desktop coffee shop companion and counter app for customers and café staff, in a warm cozy patisserie-café desktop style.

`Dial: ENERGY 2 / RHYTHM 2 / MOTION 1`

- **ENERGY 2 (Balanced):** Warm café cream canvas and espresso brown hierarchy paired with a measured strawberry-milk accent on primary culinary moments, approachable without loud marketing visuals.
- **RHYTHM 2 (Consistent with intentional breaks):** Fixed desktop navigation shell paired with a two-pane master-detail menu workspace (photo-forward product cards on the left, calm pinned item detail and in-context staff actions on the right), avoiding repetitive 4-card KPI dashboards.
- **MOTION 1 (Calm desktop responsiveness):** Subtle hover/focus transitions and instant async state feedback only; zero continuous loops, bouncing elements, or decorative animations.

### Liveliness Levers
1. **One focal point per screen:** In the shell, the active feature workspace; in Café Menu Gallery, the selected item's full photo and detail pane alongside the scannable photo grid.
2. **Hierarchical contrast:** Size, weight, and warm ink depth separate page titles (`16pt Bold`), item prices/names (`11pt Bold`), and secondary metadata (`9pt Regular`).
3. **Structural whitespace:** Generous `16px-24px` workspace margins and `12px-16px` card padding prevent crowded desktop forms.
4. **One deliberate accent:** Strawberry Milk (`#B85467` / `#FBECEF`) highlights selected category filters and customization tags instead of coloring every border and button.
5. **Identity motif:** Warm oat-paper framing with crisp `1px` almond/roast borders, espresso rule dividers, and a minimal custom CuppaClub cup mark in the sidebar header (no generic clip art or emoji).

---

## Color Palette (WCAG AA & Non-Text Contrast Verified)

Limited to neutral cream/milk surfaces, 2 core café brown/taupe tones, 1 strawberry-milk accent, and semantic status tones:

| Role | Hex | Usage | Contrast Verification |
|---|---|---|---|
| Canvas (Vanilla Cream) | `#FAF4EC` | Main workspace background | 12.98:1 with `#3B261C` |
| Surface (Warm Milk) | `#FFFDF9` | Cards, detail panel, modal forms, inputs | 13.97:1 with `#3B261C` |
| Sidebar (Soft Oat) | `#F3E8DC` | Left navigation rail and subtle inset wells | 11.75:1 with `#3B261C` |
| Divider Line (Almond) | `#E2D3C3` | Non-interactive structural dividers and card framing | Visual separation only |
| Control Border (Roasted Taupe) | `#967B65` | Input borders, secondary button outlines, interactive edges | 3.89:1 on Surface, 3.61:1 on Canvas (PASS >= 3.0:1) |
| Text Primary (Dark Espresso) | `#3B261C` | Headings, item titles, prices, primary labels | >= 11.75:1 across all surfaces (PASS) |
| Text Secondary (Roast Muted) | `#6E5647` | Descriptions, metadata, owner captions | 6.23:1 on Canvas, 6.70:1 on Surface (PASS) |
| Primary Interactive (Mocha) | `#6B442E` | Primary buttons, focus indicators, active border highlight | 8.30:1 with `#FFFDF9`, 7.71:1 focus vs Canvas (PASS) |
| Primary Hover (Dark Roast) | `#553523` | Hover/pressed state for primary actions | 10.77:1 with `#FFFDF9` (PASS) |
| Accent (Strawberry Milk) | `#B85467` | Active filter emphasis and deliberate café accent | 4.59:1 with `#FFFDF9` (PASS) |
| Accent Tint / Text | `#FBECEF` / `#8F3547` | Customization indicator tag and warm soft highlight | 6.63:1 (PASS) |
| Positive / Available (Sage) | `#EAF2EC` / `#2F4F38` | Status `Available` / `Tersedia` / `Terhubung` | 8.02:1 (PASS); paired with text label |
| Coming Soon / Neutral (Taupe) | `#EFE7DF` / `#5C4E43` | Status `Coming Soon` / `Nonaktif` | 6.54:1 (PASS); paired with text label |
| Warning / Unconfigured | `#FDF3E3` / `#7A460E` | `Belum Dikonfigurasi` / confirmation notices | 7.05:1 (PASS); paired with text label |
| Error / Unavailable | `#FCEBEC` / `#852931` | `Habis` / validation & network errors | 7.73:1 (PASS); paired with text label |

---

## Typography

- **Font Family:** `Segoe UI` (native Windows UI typeface supporting `PerMonitorV2` High-DPI scaling cleanly without external font dependencies).
- **Scale:**
  - Brand Title: `14pt Bold`
  - Page Heading: `16pt Bold`
  - Section / Detail Heading: `12pt Bold`
  - Card Title & Price: `10.5pt Bold`
  - Body, Inputs, Nav Items: `9.5pt Regular / SemiBold`
  - Captions & Status Labels: `8.5pt SemiBold`

---

## Layout, Spacing & Controls

- **Window Dimensions:** Default `1200 x 760`, minimum supported `1024 x 680`, centered on screen.
- **Shell Structure:**
  - Left Navigation Sidebar (`248px` fixed width): CuppaClub header mark, the 5 main features driven directly from `FeatureRegistry` (each displaying its live status label: `Tersedia`, `Coming Soon`, or `Nonaktif`), account panel (`Masuk` / active user + role + `Keluar`), and `Koneksi Supabase` status trigger.
  - Top Context Header (`64px` height): Active feature title, owner/status subtitle, and live backend readiness indicator.
  - Main Content Host: Hosts the active feature `UserControl` (`ComingSoonControl` for unshipped features; `MenuGalleryControl` for Feature 01).
- **Radius Discipline:** Controlled `6px` radius on buttons/inputs/tags and `8px` on cards/panels; never pill-shaped across every control.
- **Photography First:** Menu item photos occupy a `16:10` visual area (`244 x 148` on cards, `288 x 190` in detail panel) with clean warm fallback framing (`"Foto belum tersedia"`) instead of generic clip-art icons.
- **Design Rationale (R-31):**
  - Palette: Warm cream, mocha brown, strawberry milk, and sage reflect the owner's sweet café brief while passing WCAG AA contrast.
  - Layout: Left navigation + master-detail view fits desktop counter/customer workflows better than a web-style landing page or fake KPI dashboard.
  - Typography: Native `Segoe UI` preserves crisp WinForms ClearType rendering and predictable keyboard focus metrics across Windows scaling levels.
  - Spacing: `8px` base unit (`8/12/16/24px`) separates interactive filters from product cards and prevents clipping on `1024x680`.
  - Cards: Used exclusively for visual menu catalog entries where photo, price, and availability must be scanned together.
  - Brand Mark: Simple GDI+ painted cup-and-coaster outline beside the `"CuppaClub"` title avoids unconfirmed external logos and emoji noise.

---

## Desktop Ergonomics & Honest States

- **Keyboard & Focus:** Logical `TabIndex` order matching visual layout, `Hand` cursor on enabled buttons/cards, `Enter`/`Space` activation on custom cards/buttons, `Escape` closes modal dialogs (`LoginForm`, `SupabaseConfigForm`, `MenuItemEditorForm`), and clear `#6B442E` focus outlines on custom controls.
- **Color Independence:** Statuses (`Tersedia`, `Habis`, `Coming Soon`, `Nonaktif`, `Terhubung`, `Belum Terhubung`) always render descriptive text alongside color.
- **Required States:** Every remote data view implements explicit `Unconfigured`, `Loading`, `Empty` (distinguishing empty database from empty filter results), `Error` (with Retry), `Success`, and `Disabled` states.
- **Zero Slop Rules:** No emojis in UI labels, no fake statistics/ratings/testimonials, no dead buttons (staff-only controls are hidden when not signed in as staff), and no duplicate design documents.
