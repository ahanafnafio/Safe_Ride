using System;
using System.Collections.Generic;
using Saferide.Models;
using BCrypt.Net;  //Adding this BCrypt library for hashing as using statement

namespace Saferide.Services
{
    public class Authentication
    {
        // Attributes
        private List<User> users; // Will be stored in database later
        private List<Session> sessions; // Will be stored in database later
        // Constructor
        public Authentication()
        {
            users = new List<User>();
            sessions = new List<Session>();
        }
        // Methods
        public User? Register(string firstName, string lastName, string email, string password, string role) // '?' allows User to be null if already exists
        {
            foreach (User user in users)
            {
                if (user.GetEmail().ToLower() == email.ToLower())
                {
                    return null; // email already exists
                }
            }

            string passwordHash = HashPassword(password);
            User newUser;
            if (role == "Rider")
            {
                newUser = new Rider(firstName, lastName, email, passwordHash);
            }
            else
            {
                newUser = new Driver(firstName, lastName, email, passwordHash);
            }
            users.Add(newUser);

            return newUser;
        }

        public Session? Login(string email, string password) // '?' allows Session to be null if login fails
        {
            foreach (User user in users)
            {
                if (user.GetEmail().ToLower() == email.ToLower())
                {
                    if (BCrypt.Net.BCrypt.Verify(password, user.GetPasswordHash())) // Useing BCrypt.Verify to check the plain text password against the stored hash
                    {
                        string sessionId = Guid.NewGuid().ToString();
                        Session newSession = new Session(sessionId, user.GetUserId());
                        sessions.Add(newSession);
                        return newSession;
                    }
                }
            }
            return null;
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
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12); //I think 12 is a good balance between security and speed
        }
    }
}