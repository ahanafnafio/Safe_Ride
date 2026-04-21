namespace Saferide.Models
{
    public class Ride
    {
        // Attributes
        private static int nextRideId = 1;
        private int rideId; //db make
        private string status;
        private string notes;
        private Location pickup;
        private Location dropoff;
        private DateTime requestedAt;
        private DateTime updatedAt;
        // riderId?
        private int vehicleId;
        private double ratingScore;
        private bool hasRating;
        // Constructor
        public Ride(Location pickup, Location dropoff, string notes, int vehicleId)
        {
            this.pickup = pickup;
            this.dropoff = dropoff;
            this.notes = notes;
            rideId = nextRideId++;
            this.vehicleId = vehicleId;

            status = "Requested";
            requestedAt = DateTime.Now;
            updatedAt = DateTime.Now;

            ratingScore = 0.0;
            hasRating = false;
        }
        // Methods
        public int GetRideId()
        {
            return rideId;
        }
        public DateTime GetRequestTime()
        {
            return requestedAt;
        }
        public string GetStatus()
        {
            return status;
        }
        public string GetNotes()
        {
            return notes;
        }
        public Location GetPickup()
        {
            return pickup;
        }
        public Location GetDropoff()
        {
            return dropoff;
        }
        public void SetStatus(string newStatus)
        {
            status = newStatus;
        }
        public void Cancel()
        {
            // Come back
            status = "Cancelled";
        }
        public bool IsRateable()
        {
            if(!hasRating && status == "Completed")
            {
                return true;
            }
            return false;
        }
        public void AddRating(double score) // Make return a boolean?
        {
            if (IsRateable() && score >= 1 && score <= 5)
            {
                ratingScore = score;
                hasRating = true;
            }
        }

    }
}