-- Optional starter seed data for menu_items (idempotent).
-- Safe for development/testing. Upload real cafe photos in Storage 'menu-photos'
-- or edit image_url via staff account in Cafe Menu Gallery.

insert into public.menu_items (name, category, description, base_price, image_url, is_available, supports_customization)
select v.name, v.category, v.description, v.base_price, v.image_url, v.is_available, v.supports_customization
from (
    values
        ('Caramel Oat Latte', 'Coffee', 'Double espresso house blend dengan oat milk hangat dan sirup karamel lembut.', 34000.00, '', true, true),
        ('Iced Aren Brown Sugar Latte', 'Coffee', 'Espresso single origin dipadukan susu segar dan gula aren Nusantara.', 30000.00, '', true, true),
        ('Strawberry Cream Matcha', 'Non-Coffee', 'Matcha ceremonial di atas susu stroberi manis dengan busa krim lembut.', 36000.00, '', true, true),
        ('Belgian Dark Choco', 'Non-Coffee', 'Cokelat hitam pekat disajikan hangat atau dingin dengan susu creamy.', 32000.00, '', true, true),
        ('Honey Butter Croissant', 'Pastry', 'Croissant renyah berlapis mentega Prancis dengan glasir madu ringan.', 26000.00, '', true, false),
        ('Cinnamon Sugar Brioche', 'Pastry', 'Roti brioche empuk dengan taburan kayu manis hangat dan gula halus.', 24000.00, '', true, false),
        ('Basque Burnt Cheesecake', 'Dessert', 'Cheesecake panggang bertekstur lembut di tengah dengan aroma vanilla.', 42000.00, '', true, false),
        ('Classic Tiramisu Cup', 'Dessert', 'Lapisan sponge espresso, mascarpone krim lembut, dan bubuk kakao.', 39000.00, '', false, false)
) as v(name, category, description, base_price, image_url, is_available, supports_customization)
where not exists (
    select 1
    from public.menu_items m
    where lower(m.name) = lower(v.name)
);
