using System;

namespace Saferide.Models
{
    public class User
    {
        // Attributes
        private int userId;
        //public static int nextId = 1;
        private string firstName;
        private string lastName;
        private string email;
        private string passwordHash;
        private string role;


        // Empty constructor for database fetches
        public User()
        {
            userId = 0;
            firstName = "";
            lastName = "";
            email = "";
            passwordHash = "";
            role = "";
        }


        // Constructor for creating a new user before storing in database
        public User(string firstName, string lastName, string email, string passwordHash, string role)
        {
            userId = 0;
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

        public void SetUserID(int id)
        {
            userId = id;
        }

        public void SetFirstName(string first)
        {
            firstName = first;
        }

        public void SetLastName(string last)
        {
            lastName = last;
        }

        public void SetEmail(string newEmail)
        {
            email = newEmail;
        }

        public void SetPasswordHash(string newHash)
        {
            passwordHash = newHash;
        }

        public void SetUserRole(string newRole)
        {
            role = newRole;
        }
    }
}