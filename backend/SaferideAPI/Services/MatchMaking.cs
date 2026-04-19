using System.Numerics;
using Saferide.Models;

namespace Saferide.Services
{
    public class MatchMaking
    {
        private List<Driver> drivers;
        private List<Ride> rides;
        private readonly RoutingService routingService;

        // Constructor
        public MatchMaking(RoutingService routingService)
        {
            drivers = new List<Driver>();
            rides = new List<Ride>();
            this.routingService = routingService;
        }

        // Methods
        public void AddDriver(Driver newDriver)
        {
            drivers.Add(newDriver);
        }
        public async Task<RideAssignmentResult?> AddRide(Ride newRide)
        {
            // Store the ride
            rides.Add(newRide);

            // Find best driver using Google Route Matrix
            DriverEtaResult? bestResult = await routingService.ComputeRouteMatrixAsync(drivers, newRide.GetPickup());

            // If no driver found, return null
            if (bestResult == null)
            {
                return null;
            }

            // Assign the driver
            Driver driver = bestResult.Driver;
            newRide.SetStatus("Assigned");
            driver.SetAvailability(false);

            // Compute the final trip route (pickup -> dropoff)
            RouteResult? tripRoute = await routingService.ComputeRouteAsync(newRide.GetPickup(), newRide.GetDropoff());

            if (tripRoute == null)
            {
                return null;
            }

            // Return assignment + route info
            return new RideAssignmentResult
            {
                RideStatus = newRide.GetStatus(),
                DriverFirstName = driver.GetFirstName(),
                DriverLastName = driver.GetLastName(),
                DriverEtaSeconds = bestResult.DurationSeconds,
                RouteDuration = tripRoute.Duration ?? "",
                RouteDistanceMeters = tripRoute.DistanceMeters,
                EncodedPolyline = tripRoute.EncodedPolyline ?? ""
            };
        }
    }
}