namespace Saferide.Models
{
    public class RouteRequest // Helper for recieving JSON (DOES NOT replace Location class)
    {
        // Starting point
        public string OriginAddress { get; set; } = "";
        public double OriginLat { get; set; }
        public double OriginLon { get; set; }

        // Ending point
        public string DestinationAddress { get; set; } = "";
        public double DestinationLat { get; set; }
        public double DestinationLon { get; set; }
    }
}