using System;
using System.Collections.Generic;
using Saferide.Models;
using Saferide.Database;
using BCrypt.Net;

namespace Saferide.Services
{
    public class Authentication
    {
        // Attributes
        private List<Session> sessions; // Will be stored in database
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
            // Search for email in database
            if (db.EmailExistsAnywhere(email))
            {
                return null;    // if true, email already exists, don't return a User object
            }

            // Hash the raw password before registering
            string passwordHash = HashPassword(password);

            if (role == "Rider") // Creating Rider
            {
                Rider newRider = new Rider(firstName, lastName, email, passwordHash);
                
                // Attempt to store rider object, return null if failed
                if (!db.ExecuteStoreNewRider(newRider))
                {
                    return null;
                }

                return newRider;
            }

            else if (role == "Driver") // Creating Driver
            {
                Driver newDriver = new Driver(firstName, lastName, email, passwordHash);
                
                // Attempt to store driver object, return null if failed
                if (!db.ExecuteStoreNewDriver(newDriver))
                {
                    return null;
                }

                return newDriver;
            }

            else
            {
                return null; // Incorrect choice
            }
        }

        public Session? Login(string email, string password) // '?' allows Session to be null if login fails
        {
            // Fetch Rider, could return null
            Rider? foundRider = db.ExecuteFetchRider(email);
            Driver? foundDriver = db.ExecuteFetchDriver(email);

            // If there was a match in Rider table, verify password
            // --> if successful, login worked, and a session is created
            if (foundRider != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, foundRider.GetPasswordHash()))
                {
                    string sessionId = Guid.NewGuid().ToString();
                    Session newSession = new Session(sessionId, foundRider.GetUserId());
                    sessions.Add(newSession);
                    return newSession;
                }

                return null;    // Return null if password did not match
            }

            // If there was a match in Driver table, verify password
            // --> if successful, login worked, and a session is created
            if (foundDriver != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, foundDriver.GetPasswordHash()))
                {
                    string sessionId = Guid.NewGuid().ToString();
                    Session newSession = new Session(sessionId, foundDriver.GetUserId());
                    sessions.Add(newSession);
                    return newSession;
                }

                return null;    // Return null if password did not match
            }

            return null;    // Return null if both fetches were unsuccessful
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
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12); // 12 is a good balance between security and speed
        }
    }
}