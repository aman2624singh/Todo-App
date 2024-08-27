using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Services
{
    public class UserSessionService : IUserSessionService
    {
        private const string UserIdKey = "LoggedInUserId";

        public int GetUserId()
        {
            return Preferences.Get(UserIdKey, 0); 
        }

        public void SetUserId(int userId)
        {
            Preferences.Set(UserIdKey, userId);
        }

        public void ClearSession()
        {
            Preferences.Remove(UserIdKey);
        }
    }

}
