using SaferideAPI.Models;
namespace SaferideAPI.Services
{
    public class Authentication
    {
        // Attributes
        private List<User> users; // Will be stored in database later
        private List<Session> sessions; // Will be stored in database later
        private int nextUserId;
        // Constructor
        public Authentication()
        {
            users = new List<User>();
            sessions = new List<Session>();
            nextUserId = 1;
        }
        // Methods
        public User? Register(string firstName, string lastName, string email, string password) // '?' allows User to be null if already exists
        {
            foreach (User user in users)
            {
                if (user.GetEmail().ToLower() == email.ToLower())
                {
                    return null; // email already exists
                }
            }

            string passwordHash = HashPassword(password);

            User newUser = new Rider(nextUserId, firstName, lastName, email, passwordHash);
            users.Add(newUser);
            nextUserId++;

            return newUser;
        }

        public Session? Login(string email, string password) // '?' allows Session to be null if login fails
        {
            string passwordHash = HashPassword(password);

            foreach (User user in users)
            {
                if (user.GetEmail().ToLower() == email.ToLower() &&
                    user.GetPasswordHash() == passwordHash)
                {
                    string sessionId = Guid.NewGuid().ToString(); // global unique identifier
                    Session newSession = new Session(sessionId, user.GetUserId());
                    sessions.Add(newSession);
                    return newSession;
                }
            }

            return null;
        }

        public void Logout(string sessionId)
        {
            foreach (Session session in sessions)
            {
                if (session.GetSessionId() == sessionId && session.IsValid())
                {
                    session.Invalidate();
                    return;
                }
            }
        }

        private string HashPassword(string password)
        {
            // TEMPORARY
            // Replace later with real hashing
            return password;
        }
    }
}