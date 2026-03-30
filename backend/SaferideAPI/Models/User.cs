using System;

namespace Saferide.Models
{
    public class User
    {
        // Attributes
        private int userId;
        public static int nextId = 1;
        private string firstName;
        private string lastName;
        private string email;
        private string passwordHash;
        private string role;

        // Constructor
        public User(string firstName, string lastName, string email, string passwordHash, string role)
        {
            userId = nextId++;
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.passwordHash = passwordHash;
            this.role = role;
        }
        // Methods
        public int GetUserId()
        {
            return userId;
        }

        public string GetFirstName()
        {
            return firstName;
        }

        public string GetLastName()
        {
            return lastName;
        }

        public string GetEmail()
        {
            return email;
        }

        public string GetPasswordHash()
        {
            return passwordHash;
        }
        
        public string GetRole()
        {
            return role;
        }

        public void SetPasswordHash(string newHash)
        {
            passwordHash = newHash;
        }
    }
}