namespace Saferide.Models
{
    public class Location
    {
        private string address;
        private double lat;
        private double lon;
        // Constructor
        public Location(string address, double lat, double lon)
        {
            this.address = address;
            this.lat = lat;
            this.lon = lon;
        }

        // Methods
        public string GetAddress()
        {
            return address;
        }
        public double GetLat()
        {
            return lat;
        }
        public double GetLon()
        {
            return lon;
        }
    }
}