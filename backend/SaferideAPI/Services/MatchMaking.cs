using System.Numerics;
using Saferide.Models;

namespace Saferide.Services
{
    public class MatchMaking
    {
        private List<Driver> drivers;
        private List<Ride> rides;

        // Construtor
        public MatchMaking()
        {
            drivers = new List<Driver>();
            rides = new List<Ride>();
        }

        // Methods
        public void AddDriver(Driver newDriver)
        {
            drivers.Add(newDriver);
        }
        public void AddRide(Ride newRide)
        {
            rides.Add(newRide);

            Driver? driver = FindClosestDriver(newRide); // When added automatically finds the closest driver

            if (driver != null)
            {
                newRide.SetStatus("Assigned");
                driver.SetAvailability(false);
            }
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double earthRadiusMiles = 3958.8;

            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadiusMiles * c;
        }
        public Driver? FindClosestDriver(Ride newRide)
        {
            Location pickup = newRide.GetPickup();

            Driver? closestDriver = null;
            double minDistance = double.MaxValue;

            foreach (Driver d in drivers)
            {
                if (!d.IsAvailable())
                {
                    continue;
                }

                Location? driverLocation = d.GetCurrentLocation();

                if (driverLocation == null)
                {
                    continue;
                }

                double distance = CalculateDistance(driverLocation.GetLat(), driverLocation.GetLon(), pickup.GetLat(), pickup.GetLon());
                Console.WriteLine($"{d.GetFirstName()} is {distance} miles away from pickup location");

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestDriver = d;
                }
            }
            return closestDriver;
        }
    }
}