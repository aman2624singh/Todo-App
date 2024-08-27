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
        void SetUserId(int userId);  
        void ClearSession();  
    }
}
