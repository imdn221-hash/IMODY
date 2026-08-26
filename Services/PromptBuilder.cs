using IMODY.Models;
using System.Text;

namespace IMODY.Services
{
    public static class PromptBuilder
    {
        public static string BuildPrompt(TravelPlan plan)
        {
            StringBuilder sb = new();

            sb.AppendLine("Sen profesyonel bir seyahat planlayıcısısın.");
            sb.AppendLine($"Gidilecek Yer: {plan.Destination}");
            sb.AppendLine($"Gidiş: {plan.DepartureDate:dd.MM.yyyy}");
            sb.AppendLine($"Dönüş: {plan.ReturnDate:dd.MM.yyyy}");
            sb.AppendLine($"Kişi Sayısı: {plan.PeopleCount}");
            sb.AppendLine($"Konaklama: {plan.Accommodation}");

            sb.AppendLine("İlgi Alanları:");

            foreach (var interest in plan.Interests)
            {
                sb.AppendLine("- " + interest);
            }

            sb.AppendLine();
            sb.AppendLine("Bana detaylı bir gezi planı oluştur.");

            return sb.ToString();
        }
    }
}