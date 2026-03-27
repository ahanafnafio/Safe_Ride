namespace Saferide.Models
{
    public class Rider : User
    {
        // Constructor
        public Rider(string firstName, string lastName, string email, string passwordHash) : base(firstName, lastName, email, passwordHash, "Rider")
        {
        }

        // Rider methods can be added later
        // public Ride RequestRide(<.../>) { }
    }
}