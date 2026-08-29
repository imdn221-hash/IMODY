using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace IMODY
{
    public class ExplorePost
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public string AuthorName { get; set; } = "";
        public bool IsOfficial { get; set; } = false;
        public string Category { get; set; } = "🏛️ Belediye / Festival"; // 🏛️ Belediye, 🎵 Konser, 🎒 Gezgin
        public string City { get; set; } = "";
        public string Content { get; set; } = "";
        public string EventDate { get; set; } = "";
        public string PriceInfo { get; set; } = "Ücretsiz";
        public int LikesCount { get; set; } = 12;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public static class PostService
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "IMODY",
            "explore_posts.json");

        private static List<ExplorePost> GetDefaultPosts()
        {
            return new List<ExplorePost>
            {
                new ExplorePost
                {
                    Title = "Travis Scott: Utopia World Tour",
                    AuthorName = "Live Nation UK",
                    IsOfficial = true,
                    Category = "🎵 Konser / Gösteri",
                    City = "Londra",
                    Content = "Travis Scott Londra O2 Arena'da sahne alıyor! Muazzam görsel şovlar ve efsanevi sahne performansı seni bekliyor.",
                    EventDate = "28 Ağustos 2026 - 20:30",
                    PriceInfo = "£85 - Ücretli",
                    LikesCount = 284
                },
                new ExplorePost
                {
                    Title = "Buca Belediyesi Açık Hava Sinema Günleri",
                    AuthorName = "Buca Belediyesi",
                    IsOfficial = true,
                    Category = "🏛️ Belediye / Festival",
                    City = "İzmir",
                    Content = "Hasanağa Bahçesi'nde çimlerin üzerinde nostaljik Türk sineması gösterimi ve ücretsiz mısır ikramı.",
                    EventDate = "Bu Akşam - 21:00",
                    PriceInfo = "Ücretsiz",
                    LikesCount = 95
                },
                new ExplorePost
                {
                    Title = "Kapadokya Vadisi Dolunay Gece Yürüyüşü & Kamp",
                    AuthorName = "Kapadokya Gezginler Kulübü",
                    IsOfficial = false,
                    Category = "🎒 Gezgin Keşfi",
                    City = "Kapadokya",
                    Content = "Dolunay ışığında peri bacaları arasında mistik yürüyüş ve gece akustik kamp ateşi dinletisi.",
                    EventDate = "Yarın - 22:00",
                    PriceInfo = "Ücretsiz",
                    LikesCount = 142
                },
                new ExplorePost
                {
                    Title = "Roma Trastevere Sokak Lezzetleri Festivali",
                    AuthorName = "Roma Kültür & Sanat Vakfı",
                    IsOfficial = true,
                    Category = "🏛️ Belediye / Festival",
                    City = "Roma",
                    Content = "Trastevere sokaklarında geleneksel el yapımı makarnalar, taze gelato ve yerel müzisyenlerin performansları.",
                    EventDate = "Hafta Sonu Boyunca",
                    PriceInfo = "Ücretsiz Giriş",
                    LikesCount = 310
                }
            };
        }

        public static List<ExplorePost> GetAll()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    var defaults = GetDefaultPosts();
                    SaveAll(defaults);
                    return defaults;
                }

                string json = File.ReadAllText(FilePath);
                var list = JsonSerializer.Deserialize<List<ExplorePost>>(json);
                if (list == null || list.Count == 0)
                {
                    list = GetDefaultPosts();
                    SaveAll(list);
                }
                return list;
            }
            catch
            {
                return GetDefaultPosts();
            }
        }

        public static List<ExplorePost> GetPosts() => GetAll();

        public static void Add(ExplorePost post)
        {
            List<ExplorePost> posts = GetAll();
            posts.Insert(0, post);
            SaveAll(posts);
        }

        public static void AddPost(ExplorePost post) => Add(post);

        private static void SaveAll(List<ExplorePost> posts)
        {
            try
            {
                string? folder = Path.GetDirectoryName(FilePath);
                if (folder != null)
                    Directory.CreateDirectory(folder);

                File.WriteAllText(FilePath, JsonSerializer.Serialize(posts, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch
            {
                // Hata yakalama
            }
        }
    }
}