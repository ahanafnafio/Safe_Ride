using Saferide.Models;
namespace Saferide.Services
{
    public class Session
    {
        // Attributes
        private string sessionId;
        private int userId;
        private bool isActive;
        // Constructor
        public Session(string sessionId, int userId)
        {
            this.sessionId = sessionId;
            this.userId = userId;
            isActive = true;
        }
        // Methods
        public string GetSessionId()
        {
            return sessionId;
        }

        public int GetUserId()
        {
            return userId;
        }

        public bool IsValid()
        {
            return isActive;
        }

        public void Invalidate()
        {
            isActive = false;
        }
    }
}