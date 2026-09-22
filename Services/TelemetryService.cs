using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IMODY
{
    public static class TelemetryService
    {
        // =====================================================================
        // CANLI GOOGLE E-TABLOLAR (EXCEL) WEBHOOK BAĞLANTISI
        // İklim Düzen'in Özel Google E-Tablosuna Satır Satır Veri Yazar
        // =====================================================================
        public static string GoogleSheetsWebhookUrl { get; set; } =
            "https://script.google.com/macros/s/AKfycbwv7_S6iRaA299MifJbNY2to1lZk_72QTuSIukLuWnPHWO06pFXoLh0OWapYawR-W8i/exec";

        private static readonly HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        /// <summary>
        /// Yeni bir kullanıcı kayıt olduğunda Google E-Tabloya satır ekler
        /// </summary>
        public static void LogUserRegistered(string name, string email)
        {
            Task.Run(async () =>
            {
                await SendToGoogleSheetsAsync(new
                {
                    time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    @event = "YENİ_KAYIT",
                    name = name,
                    email = email,
                    details = "Kayıt Başarılı (KVKK Onaylandı)",
                    os = Environment.OSVersion.ToString()
                });
            });
        }

        /// <summary>
        /// Bir kullanıcı uygulamada oturum açtığında Google E-Tabloya satır ekler
        /// </summary>
        public static void LogUserLogin(string name, string email)
        {
            Task.Run(async () =>
            {
                await SendToGoogleSheetsAsync(new
                {
                    time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    @event = "OTURUM_AÇILDI",
                    name = name,
                    email = email,
                    details = "Uygulama Açıldı / Giriş Yapıldı",
                    os = Environment.OSVersion.ToString()
                });
            });
        }

        /// <summary>
        /// Kullanıcı bir rota planı oluşturduğunda (hedef şehir, gün, kişi sayısı, konaklama) Google E-Tabloya yazar
        /// </summary>
        public static void LogTripCreated(string destination, int days, int peopleCount, string accommodation)
        {
            Task.Run(async () =>
            {
                string userName = string.IsNullOrWhiteSpace(UserSession.Name) ? "Misafir Gezgin" : UserSession.Name;
                string userEmail = string.IsNullOrWhiteSpace(UserSession.Email) ? "Bilinmiyor" : UserSession.Email;

                await SendToGoogleSheetsAsync(new
                {
                    time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    @event = "YENİ_ROTA_PLANI",
                    name = userName,
                    email = userEmail,
                    details = $"Hedef: {destination} | {days} Gün | {peopleCount} Kişi | {accommodation}",
                    os = Environment.OSVersion.ToString()
                });
            });
        }

        private static async Task SendToGoogleSheetsAsync(object dataPayload)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GoogleSheetsWebhookUrl))
                    return;

                string json = JsonSerializer.Serialize(dataPayload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Asenkron istek - kullanıcı arayüzünü asla bekletmez veya dondurmaz
                await httpClient.PostAsync(GoogleSheetsWebhookUrl, content);
            }
            catch
            {
                // Telemetri hatası kullanıcı deneyimini etkilemez
            }
        }
    }
}
