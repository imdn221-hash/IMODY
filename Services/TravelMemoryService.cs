using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using IMODY.Models;

namespace IMODY.Services
{
    public class TravelMemoryService
    {
        private static readonly string AppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "IMODY"
        );

        private static readonly string PhotosFolder = Path.Combine(AppDataFolder, "AlbumPhotos");
        private static readonly string DataFilePath = Path.Combine(AppDataFolder, "travel_memories.json");

        static TravelMemoryService()
        {
            try
            {
                if (!Directory.Exists(AppDataFolder))
                    Directory.CreateDirectory(AppDataFolder);

                if (!Directory.Exists(PhotosFolder))
                    Directory.CreateDirectory(PhotosFolder);
            }
            catch { }
        }

        public static List<TravelMemory> LoadMemories()
        {
            try
            {
                if (File.Exists(DataFilePath))
                {
                    string json = File.ReadAllText(DataFilePath);
                    var list = JsonSerializer.Deserialize<List<TravelMemory>>(json);
                    if (list != null && list.Count > 0)
                        return list;
                }
            }
            catch { }

            // Eğer kayıt yoksa örnek 2 adet seyahat oluşturalım
            var sampleList = GetInitialSampleMemories();
            SaveMemories(sampleList);
            return sampleList;
        }

        public static void SaveMemories(List<TravelMemory> memories)
        {
            try
            {
                if (!Directory.Exists(AppDataFolder))
                    Directory.CreateDirectory(AppDataFolder);

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(memories, options);
                File.WriteAllText(DataFilePath, json);
            }
            catch { }
        }

        public static string CopyPhotoToAlbum(string sourcePath)
        {
            try
            {
                if (!File.Exists(sourcePath))
                    return sourcePath;

                if (!Directory.Exists(PhotosFolder))
                    Directory.CreateDirectory(PhotosFolder);

                string ext = Path.GetExtension(sourcePath);
                string newFileName = $"photo_{Guid.NewGuid()}{ext}";
                string destPath = Path.Combine(PhotosFolder, newFileName);

                File.Copy(sourcePath, destPath, true);
                return destPath;
            }
            catch
            {
                return sourcePath;
            }
        }

        private static List<TravelMemory> GetInitialSampleMemories()
        {
            return new List<TravelMemory>
            {
                new TravelMemory
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Roma & Vatikan Keşfi",
                    Destination = "Roma, İtalya",
                    DateRange = "14 - 19 Nisan 2026",
                    Rating = 5,
                    Notes = "Trastevere bölgesindeki küçük makarna lokantaları hayatımın en lezzetli carbonarasını sunuyordu. Sabah 08:00'de Colosseum'a gitmek kalabalıktan kaçmak için harika bir fikirdi. Gün batımında Pincio terasından manzarayı izlemek unutulmazdı!",
                    Photos = new List<string>(),
                    CreatedAt = DateTime.Now.AddDays(-20)
                },
                new TravelMemory
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Tokyo Sakura & Kültür Gezisi",
                    Destination = "Tokyo, Japonya",
                    DateRange = "28 Mart - 5 Nisan 2026",
                    Rating = 5,
                    Notes = "Shinjuku Gyoen bahçesinde kiraz çiçeklerinin altında piknik yaptık. Akşamları Shibuya Crossing'in enerjisi ve Shinjuku ara sokaklarındaki ramen dükkanları büyüleyiciydi. Ulaşımda Suica kart hayat kurtardı.",
                    Photos = new List<string>(),
                    CreatedAt = DateTime.Now.AddDays(-60)
                }
            };
        }
    }
}
