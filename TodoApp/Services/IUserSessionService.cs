using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Services
{
    public interface IUserSessionService
    {
        int GetUserId();      
        string GetUserName();
        void SetUserId(int userId);  
        void SetUserName(string userName);
        void ClearSession();

        void SetIsAdmin(bool isAdmin);
        bool IsAdmin();
    }
}
