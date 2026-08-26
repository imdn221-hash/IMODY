using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IMODY
{
    public static class ThemeManager
    {
        public static void ApplyTheme(string themeName)
        {
            var app = Application.Current;
            if (app == null) return;

            UserSession.CurrentTheme = themeName;
            UserSession.SaveSession();

            Color bgPrimary, bgCard, bgSub, border, accent, textPrimary, textSecondary;

            switch (themeName)
            {
                case "Cherry":
                    // VİŞNE ÇÜRÜĞÜ (İmza İMODY Teması)
                    bgPrimary     = Color.FromRgb(43, 7, 23);     // #2B0717 Vişne
                    bgCard        = Color.FromRgb(59, 12, 30);    // #3B0C1E Bordo Kart
                    bgSub         = Color.FromRgb(74, 16, 38);    // #4A1026 Koyu Bordo
                    border        = Color.FromRgb(125, 64, 83);   // #7D4053 Pembe/Bordo Sınır
                    accent        = Color.FromRgb(214, 164, 90);  // #D6A45A Şampanya Altın
                    textPrimary   = Color.FromRgb(245, 233, 221); // #F5E9DD Krem Beyaz
                    textSecondary = Color.FromRgb(217, 166, 168); // #D9A6A8 Pudra/Vişne
                    break;

                case "Dark":
                    // KOYU MOD: Simsiyah derin arka planlar, antrasit kartlar, beyaz yazılar
                    bgPrimary     = Color.FromRgb(0, 0, 0);       // #000000 Tam Siyah
                    bgCard        = Color.FromRgb(18, 18, 22);    // #121216 Koyu Antrasit
                    bgSub         = Color.FromRgb(28, 28, 36);    // #1C1C24 Koyu Gri
                    border        = Color.FromRgb(48, 48, 62);    // #30303E Sınır
                    accent        = Color.FromRgb(214, 164, 90);  // #D6A45A Altın
                    textPrimary   = Color.FromRgb(255, 255, 255); // #FFFFFF Bembeyaz
                    textSecondary = Color.FromRgb(160, 165, 180); // #A0A5B4 Açık Gri
                    break;

                case "Light":
                    // AÇIK MOD: Bembeyaz arka planlar, temiz açık gri kartlar, koyu okunaklı yazılar
                    bgPrimary     = Color.FromRgb(255, 255, 255); // #FFFFFF
                    bgCard        = Color.FromRgb(244, 246, 249); // #F4F6F9
                    bgSub         = Color.FromRgb(233, 236, 241); // #E9ECEF
                    border        = Color.FromRgb(208, 213, 221); // #D0D5DD
                    accent        = Color.FromRgb(184, 130, 48);  // #B88230 Altın/Bronz
                    textPrimary   = Color.FromRgb(17, 24, 39);    // #111827 Koyu Siyah
                    textSecondary = Color.FromRgb(75, 85, 99);    // #4B5563 Gri
                    break;

                case "Navy":
                default:
                    // KOZMİK LACİVERT (Varsayılan Otomatik Tema)
                    bgPrimary     = Color.FromRgb(11, 19, 43);    // #0B132B Derin Gece Laciverti
                    bgCard        = Color.FromRgb(28, 37, 65);    // #1C2541 Koyu Mavi Kart
                    bgSub         = Color.FromRgb(35, 49, 82);    // #233152
                    border        = Color.FromRgb(58, 80, 107);   // #3A506B
                    accent        = Color.FromRgb(214, 164, 90);  // #D6A45A Şampanya Altın
                    textPrimary   = Color.FromRgb(255, 255, 255); // #FFFFFF Parlak Beyaz
                    textSecondary = Color.FromRgb(152, 168, 198); // #98A8C6 Buz Mavisi/Gri
                    break;
            }

            var brushPrimary = new SolidColorBrush(bgPrimary);
            var brushCard = new SolidColorBrush(bgCard);
            var brushSub = new SolidColorBrush(bgSub);
            var brushBorder = new SolidColorBrush(border);
            var brushAccent = new SolidColorBrush(accent);
            var brushTextPrimary = new SolidColorBrush(textPrimary);
            var brushTextSecondary = new SolidColorBrush(textSecondary);

            app.Resources["Theme_BgPrimary"] = brushPrimary;
            app.Resources["Theme_BgCard"] = brushCard;
            app.Resources["Theme_BgSub"] = brushSub;
            app.Resources["Theme_Border"] = brushBorder;
            app.Resources["Theme_Accent"] = brushAccent;
            app.Resources["Theme_TextPrimary"] = brushTextPrimary;
            app.Resources["Theme_TextSecondary"] = brushTextSecondary;

            // Açık olan tüm pencerelerin arka planını ve içeriklerini anında güncelle
            foreach (Window win in app.Windows)
            {
                if (win == null) continue;
                win.Background = brushPrimary;
            }
        }
    }
}
