using Library.Core.Models;

namespace Library.Web.Services
{
    public class UserSessionService
    {
        private User? _currentUser;
        
        public User? CurrentUser 
        { 
            get => _currentUser;
            private set
            {
                _currentUser = value;
                OnUserChanged?.Invoke();
            }
        }

        public bool IsAuthenticated => CurrentUser != null;
        
        public bool IsAdmin => CurrentUser?.Role == UserRole.Administrator;
        
        public bool IsSupervisor => CurrentUser?.Role == UserRole.Supervisor;

        public event Action? OnUserChanged;

        public void SetUser(User user)
        {
            CurrentUser = user;
        }

        public void ClearUser()
        {
            CurrentUser = null;
        }

        public string GetUserDisplayName()
        {
            return CurrentUser?.Name ?? "Guest";
        }

        public string GetUserRole()
        {
            return CurrentUser?.Role.ToString() ?? "None";
        }
    }
}
