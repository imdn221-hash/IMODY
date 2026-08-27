using System;
using System.Collections.Generic;

namespace IMODY.Models
{
    public class TravelMemory
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string DateRange { get; set; } = string.Empty;
        public int Rating { get; set; } = 5;
        public string Notes { get; set; } = string.Empty;
        public List<string> Photos { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // UI Yardımcı Özellikler
        public string CoverPhoto => (Photos != null && Photos.Count > 0) ? Photos[0] : string.Empty;
        public int PhotoCount => Photos?.Count ?? 0;
        public string PhotoCountText => $"{PhotoCount}/5 Fotoğraf";
        public string StarRatingText => new string('★', Math.Clamp(Rating, 1, 5)) + new string('☆', 5 - Math.Clamp(Rating, 1, 5));
    }
}
