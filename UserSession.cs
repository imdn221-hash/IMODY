using System;
using System.IO;
using System.Text.Json;

namespace IMODY
{
    public static class UserSession
    {
        public static string Name { get; set; } = "Gezgin";
        public static string Email { get; set; } = "gezgin@imody.ai";
        public static string ProfileImagePath { get; set; } = "";
        public static int TravelPoints { get; set; } = 1450;
        public static string RankTitle { get; set; } = "🌍 Usta Gezgin";
        public static string CurrentTheme { get; set; } = "Navy";
        public static int CompletedTripsCount { get; set; } = 6;
        public static int CitiesDiscoveredCount { get; set; } = 12;
        public static bool HasSeenRankIntro { get; set; } = false;

        public static string GetCurrentRankTitle()
        {
            if (TravelPoints < 500) return "🎒 Çaylak Gezgin (Gezgin I)";
            if (TravelPoints < 1500) return "🏛️ Şehir Kaşifi (Gezgin II)";
            if (TravelPoints < 3000) return "⛺ Doğa & Kampçı (Gezgin III)";
            if (TravelPoints < 6000) return "🌍 Dünya Seyyahı (Gezgin IV)";
            return "👑 Efsane Gezgin (Gezgin V)";
        }

        private static string SessionFilePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IMODY", "session.json");

        public static void SaveSession()
        {
            try
            {
                var dir = Path.GetDirectoryName(SessionFilePath);
                if (!Directory.Exists(dir) && dir != null)
                    Directory.CreateDirectory(dir);

                var data = new SessionData
                {
                    Name = Name,
                    Email = Email,
                    ProfileImagePath = ProfileImagePath,
                    TravelPoints = TravelPoints,
                    RankTitle = GetCurrentRankTitle(),
                    CurrentTheme = CurrentTheme,
                    CompletedTripsCount = CompletedTripsCount,
                    CitiesDiscoveredCount = CitiesDiscoveredCount,
                    HasSeenRankIntro = HasSeenRankIntro
                };

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SessionFilePath, json);
            }
            catch { }
        }

        public static bool TryLoadSession()
        {
            try
            {
                if (File.Exists(SessionFilePath))
                {
                    string json = File.ReadAllText(SessionFilePath);
                    var data = JsonSerializer.Deserialize<SessionData>(json);
                    if (data != null && !string.IsNullOrWhiteSpace(data.Email))
                    {
                        Name = data.Name;
                        Email = data.Email;
                        ProfileImagePath = data.ProfileImagePath ?? "";
                        TravelPoints = data.TravelPoints;
                        RankTitle = data.RankTitle ?? GetCurrentRankTitle();
                        CurrentTheme = data.CurrentTheme ?? "Dark";
                        CompletedTripsCount = data.CompletedTripsCount;
                        CitiesDiscoveredCount = data.CitiesDiscoveredCount;
                        HasSeenRankIntro = data.HasSeenRankIntro;
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public static void ClearSession()
        {
            try
            {
                Name = "Gezgin";
                Email = "";
                HasSeenRankIntro = false;
                if (File.Exists(SessionFilePath))
                    File.Delete(SessionFilePath);
            }
            catch { }
        }

        private class SessionData
        {
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public string ProfileImagePath { get; set; } = "";
            public int TravelPoints { get; set; }
            public string RankTitle { get; set; } = "";
            public string CurrentTheme { get; set; } = "Dark";
            public int CompletedTripsCount { get; set; }
            public int CitiesDiscoveredCount { get; set; }
            public bool HasSeenRankIntro { get; set; }
        }
    }
}