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
            Console.WriteLine($"{newDriver.GetFirstName()} has been added to the list.");
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

            Location? driverLocation = driver.GetCurrentLocation();
            if (driverLocation == null)
            {
                return null;
            }
            // Getting driver -> pickup route
            RouteResult? driverToPickupRoute = await routingService.ComputeRouteAsync(driverLocation, newRide.GetPickup());

            if (driverToPickupRoute == null)
            {
                return null;
            }
             // set staus and turn off availability
            newRide.SetStatus("Assigned");
            driver.SetAvailability(false);

            // Return assignment + route info
            return new RideAssignmentResult
            {
                RideId = newRide.GetRideId(), // RIDEID TO KNOW WHERE TO PICK OFF
                RideStatus = newRide.GetStatus(),
                DriverFirstName = driver.GetFirstName(),
                DriverLastName = driver.GetLastName(),
                DriverEtaSeconds = bestResult.DurationSeconds,
                RouteDuration = driverToPickupRoute.Duration ?? "", // FROM DRIVER TO PICKUP
                RouteDistanceMeters = driverToPickupRoute.DistanceMeters, // FROM DRIVER TO PICKUP
                EncodedPolyline = driverToPickupRoute.EncodedPolyline ?? "" // FROM DRIVER TO PICKUP
            };
        }

        public Ride? GetRide(int rideId)
        {
            Ride? currentRide = null;
            foreach (Ride r in rides)
            {
                if (rideId == r.GetRideId())
                {
                    currentRide = r;
                    break;
                }
            }
            if (currentRide == null)
            {
                return null;
            }
            return currentRide;
        }

        public async Task<RideAssignmentResult?> CalculateFinalRoute(int rideId) // Called via driver arrived endpoint ("Driver Arrived" button in ride status on DB)
        {
            Ride? currentRide = null;
            foreach (Ride r in rides)
            {
                if (rideId == r.GetRideId())
                {
                    currentRide = r;
                    break;
                }
            }
            
            if (currentRide == null)
            {
                return null;
            }

            RouteResult? tripRoute = await routingService.ComputeRouteAsync(currentRide.GetPickup(), currentRide.GetDropoff());

            if (tripRoute == null)
            {
                return null;
            }

            currentRide.SetStatus("InProgress"); // once the ride is completed, we set it to completed
            
            return new RideAssignmentResult
            {
                RideId = currentRide.GetRideId(),
                RideStatus = currentRide.GetStatus(),
                RouteDuration = tripRoute.Duration ?? "",
                RouteDistanceMeters = tripRoute.DistanceMeters,
                EncodedPolyline = tripRoute.EncodedPolyline ?? ""
            };

        }
    }
}