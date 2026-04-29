namespace Saferide.Models
{
    public class RideAssignmentResult
    {
        public int RideId { get; set; }
        public string RideStatus { get; set; } = "";
        public string DriverFirstName { get; set; } = "";
        public string DriverLastName { get; set; } = "";
        public int DriverEtaSeconds { get; set; }

        // Final trip route info
        public string RouteDuration { get; set; } = "";
        public int RouteDistanceMeters { get; set; }
        public string EncodedPolyline { get; set; } = "";
    }
}