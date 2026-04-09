using Microsoft.AspNetCore.Mvc.Razor;
using Saferide.Services;

namespace Saferide.Models
{
    public class Driver : User
    {
        // Attributes
        private bool available;
        private double avgRating;
        private int ratingCount;
        private Location? currentLocation;
        // Constructor
        public Driver(string firstName, string lastName, string email, string passwordHash) : base(firstName, lastName, email, passwordHash, "Driver")
        {
            available = false;
            avgRating = 0.0;
            ratingCount = 0;
            currentLocation = null;
        }
        // Methods
        public bool IsAvailable()
        {
            return available;
        }

        public void GoOnline(string address, double lat, double lon) // endpoint will do Driver newDriver = new Driver("params");
        {                                                           // Then call newDriver.GoOnline("params");
            currentLocation = new Location(address, lat, lon);     // inject MatchMaking into controller-> _matchmaking.AddDriver(newDriver); or can put in GoOnline();
            available = true;
        }

        public void GoOffline()
        {
            available = false;
            currentLocation = null;
        }

        public void SetAvailability(bool isAvailable)
        {
            available = isAvailable;
        }

        public double GetAvgRating()
        {
            return avgRating;
        }

        public int GetRatingCount()
        {
            return ratingCount;
        }

        public void UpdateRating(double score)
        {
            if(score >= 1 && score <= 5)
            {
                avgRating = ((avgRating * ratingCount) + score) / (ratingCount + 1);
                ratingCount++;
            }
        }
        public void UpdateLocation(string address, double lat, double lon)
        {
            if(available) // prevents drivers not available from updating location
            {
                currentLocation = new Location(address, lat, lon);
            }
        }
        public Location? GetCurrentLocation()
        {
            return currentLocation;
        }
    }
}