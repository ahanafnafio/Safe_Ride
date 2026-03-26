using Microsoft.AspNetCore.Mvc.Razor;

namespace Saferide.Models
{
    public class Driver : User
    {
        // Attributes
        private bool available;
        private double avgRating;
        private int ratingCount;
        // Constructor
        public Driver(int userId, string firstName, string lastName, string email, string passwordHash) : base(userId, firstName, lastName, email, passwordHash, "Driver")
        {
            available = true;
            avgRating = 0.0;
            ratingCount = 0;
        }
        // Methods
        public bool IsAvailable()
        {
            return available;
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
            avgRating = ((avgRating * ratingCount) + score) / (ratingCount + 1);
            ratingCount++;
        }
    }
}