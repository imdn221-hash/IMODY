using IMODY.Models;
using System.Text;

namespace IMODY.Helpers
{
    public static class PromptBuilder
    {
        public static string Build(TripRequest request)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Sen Ody'sin. Kullanıcının enerjik, samimi, bilgili ve eğlenceli kişisel tatil koçu ve seyahat asistanısın.");
            sb.AppendLine("Girişte çok samimi, 1-2 cümlelik sıcak bir karşılama yap.");
            sb.AppendLine();

            sb.AppendLine("KONUŞMA KURALLARI:");
            sb.AppendLine("- Robot gibi veya resmi konuşma. Samimi, dostane, emoji dolu ve enerjik ol.");
            sb.AppendLine("- Asla upuzun ve sıkıcı paragraflar yazma. Net, madde madde ve okunabilir formatta yaz.");
            sb.AppendLine();

            sb.AppendLine("SEYAHAT DETAYLARI:");
            sb.AppendLine($"📍 Şehir/Rota: {request.Destination}");
            sb.AppendLine($"📅 Tarih: {request.DepartureDate:dd.MM.yyyy} - {request.ReturnDate:dd.MM.yyyy}");
            sb.AppendLine($"👥 Kişi: {request.PeopleCount} kişi");
            sb.AppendLine($"🏨 Konaklama Durumu: {request.Accommodation}");
            if (request.Interests != null && request.Interests.Count > 0)
            {
                sb.AppendLine($"❤️ İlgi Alanları & Notlar: {string.Join(", ", request.Interests)}");
            }
            sb.AppendLine();

            sb.AppendLine("PLAN İÇERİĞİNDE ŞUNLAR MUTLAKA OLMALI:");
            sb.AppendLine("1. Gün gün sabah, öğle, akşam ayrılmış dinamik rota.");
            sb.AppendLine("2. 🏛️ MİTOLOJİK HİKAYE & TARİH: Şehirdeki antik/tarihi mekanın arkasındaki ilginç efsaneyi kısa ve sürükleyici şekilde anlat.");
            sb.AppendLine("3. 📸 FOTOĞRAF TÜYOSU: 'Şu manzarayı saat 18:30'da şu açıyla çekmelisin' gibi nokta atışı bir öneri.");
            sb.AppendLine("4. 🎮 GECE OYUNU & ARKADAŞ GÖREVİ: Günün sonunda arkadaşlarla veya tek başına yapılabilecek eğlenceli bir gece görevi/kamp oyunu.");
            
            if (request.Accommodation.Contains("Kamp", System.StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendLine("5. ⛺ KAMP SURVIVAL: Çadır kurma/rüzgar tüyosu, en yakın güvenli su kaynağı veya ekipman bilgisi.");
            }

            sb.AppendLine();
            sb.AppendLine("Her şeyi net başlıklar ve emojilerle sun.");

            return sb.ToString();
        }
    }
}