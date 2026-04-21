using Saferide.Services;

namespace Saferide.Models
{
    public class Rider : User
    {
        private List<Vehicle> vehicles;
        // Constructor
        public Rider(string firstName, string lastName, string email, string passwordHash) : base(firstName, lastName, email, passwordHash, "Rider")
        {
            vehicles = new List<Vehicle>();
        }
        // Methods
        public void AddVehicle(string make, string model, string color, string plate, string notes)
        {
            Vehicle newVehicle = new Vehicle(make, model, color, plate, notes);
            vehicles.Add(newVehicle);
        }

        public bool RemoveVehicle(int vehicleId)
        {
            for (int i = 0; i < vehicles.Count; i++)
            {
                if (vehicles[i].GetVehicleId() == vehicleId)
                {
                    vehicles.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public List<Vehicle> GetVehicles()
        {
            return vehicles;
        }
        public Vehicle? GetVehicleById(int vehicleId)
        {
            foreach (Vehicle vehicle in vehicles)
            {
                if (vehicle.GetVehicleId() == vehicleId)
                {
                    return vehicle;
                }
            }
            return null;
        }

        public Ride? RequestRide(int vehicleId, string pickupAddress, double pickupLat, double pickupLon, string dropoffAddress, double dropoffLat, double dropoffLon, string notes)
        {
            Vehicle? selectedVehicle = GetVehicleById(vehicleId);

            if (selectedVehicle == null)
            {
                return null;
            }

            Location pickup = new Location(pickupAddress, pickupLat, pickupLon);
            Location dropoff = new Location(dropoffAddress, dropoffLat, dropoffLon);

            Ride newRide = new Ride(pickup, dropoff, notes, selectedVehicle.GetVehicleId());
            return newRide;
            /*Location pickup = new Location(pickupAddress, pickupLat, pickupLon);
            Location dropoff = new Location(dropoffAddress, dropoffLat, dropoffLon);
            Ride newRide = new Ride(pickup, dropoff, notes, -1); // -1= vehicle id for now (Get rid of vehicleId)
            return newRide; // endpoint will send this to matchmaking to add to list and process (STAY)*/
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