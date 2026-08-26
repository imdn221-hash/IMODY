namespace IMODY.Models
{
    public class TripRequest
    {
        public string Destination { get; set; } = "";

        public DateTime? DepartureDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public int PeopleCount { get; set; }

        public string Accommodation { get; set; } = "";

        public List<string> Interests { get; set; } = new();
    }
}