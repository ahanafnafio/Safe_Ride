using Saferide.Services;

namespace Saferide.Models
{
    public class Rider : User
    {
        // Constructor
        public Rider(string firstName, string lastName, string email, string passwordHash) : base(firstName, lastName, email, passwordHash, "Rider")
        {
        }

        public Ride RequestRide(string pickupAddress, double pickupLat, double pickupLon, string dropoffAddress, double dropoffLat, double dropoffLon, string notes)
        {
            Location pickup = new Location(pickupAddress, pickupLat, pickupLon);
            Location dropoff = new Location(dropoffAddress, dropoffLat, dropoffLon);
            Ride newRide = new Ride(pickup, dropoff, notes, -1); // -1= vehicle id for now
            return newRide; // endpoint will send this to matchmaking to add to list and process (STAY)
        }
        public void ConfirmArrival(Ride ride)
        {
            // Matchmaking will handle "Assigned"
            ride.SetStatus("DriverArrived");
        }
        public void ConfirmCompletion(Ride ride)
        {
            ride.SetStatus("Completed");
        }
        
    }
}