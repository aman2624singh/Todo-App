using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Services
{
    public class AdminAuthenticationService : IAdminAuthenticationService
    {
        private const string AdminUsername = "admin";
        private const string AdminPassword = "admin123";

        public bool IsAdmin(string username, string password)
        {
            return username == AdminUsername && password == AdminPassword;
        }
    }
}
