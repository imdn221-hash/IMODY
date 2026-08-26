using IMODY.Configuration;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace IMODY.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _apiKey;

        public GeminiService()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<GeminiService>(optional: true)
                .AddEnvironmentVariables()
                .Build();

            AppSettings settings = config.Get<AppSettings>() ?? new AppSettings();
            _apiKey = settings.Gemini.ApiKey;

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _apiKey = config["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? string.Empty;
            }
        }

        public async Task<string> GenerateAsync(string prompt)
        {
            var url =
     $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash-lite:generateContent?key={_apiKey}";

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);

            var response = await _httpClient.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json"));

            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"HTTP {(int)response.StatusCode}\n\n{responseText}";
            }

            using JsonDocument doc = JsonDocument.Parse(responseText);

            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "Yanıt alınamadı.";
        }
    }
}