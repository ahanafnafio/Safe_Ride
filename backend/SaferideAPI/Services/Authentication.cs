using System;
using System.Collections.Generic;
using Saferide.Models;
using Saferide.Database;
using System.Reflection.Metadata;

namespace Saferide.Services
{
    public class Authentication
    {
        // Attributes
        
        private List<Session> sessions; // Will be stored in database later
        private SQLiteCommunication db;
        // Constructor
        public Authentication()
        {
            sessions = new List<Session>();
            db = new SQLiteCommunication();
        }
        // Methods
        public User? Register(string firstName, string lastName, string email, string password, string role) // '?' allows User to be null if already exists
        {
            // Create temp User object
            string passwordHash = HashPassword(password);
            User newUser = new User(firstName, lastName, email, passwordHash, role);

            /*
            if (role == "Rider")
            {
                newUser = new Rider(firstName, lastName, email, passwordHash);
            }
            else
            {
                newUser = new Driver(firstName, lastName, email, passwordHash);
            }
            users.Add(newUser);
            */

        // Check if email exists, return null if the database already has it
        bool emailExists = db.ExecuteEmailLookupQuery(newUser);
        if (emailExists == true)
            {
                return null;
            }

        // Store new user in database
        bool userStored = db.ExecuteStoreNewUserQuery(newUser);
        if (userStored)
            {
                return newUser;
            }

            // store unsuccessful, return null
            return null;
        }

        public Session? Login(string email, string password) // '?' allows Session to be null if login fails
        {
            string passwordHash = HashPassword(password);
            
            // Create temp User object to send email to database query
            User tempUser = new User();
            tempUser.SetEmail(email);

            // Execute query, return User object if email found, or null if not
            User? foundUser = db.ExecuteFetchUserQuery(tempUser);

            // If query didn't return user object, it failed to locate the email in the database
            // --> return null
            if (foundUser == null)
            {
                return null;
            }

            if (foundUser.GetPasswordHash() != passwordHash)
            {
                return null;
            }

            /*
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
            */

            string sessionId = Guid.NewGuid().ToString(); // global unique identifier
            Session newSession = new Session(sessionId, foundUser.GetUserId());
            sessions.Add(newSession);
            return newSession;
        }

        public bool Logout(string sessionId) // was void
        {
            foreach (Session session in sessions)
            {
                if (session.GetSessionId() == sessionId && session.IsValid())
                {
                    session.Invalidate();
                    return true;
                }
            }
            return false;
        }

        private string HashPassword(string password)
        {
            // TEMPORARY
            // Replace later with real hashing
            return password;
        }
    }
}