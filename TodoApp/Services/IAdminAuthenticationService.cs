using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Services
{
    public interface IAdminAuthenticationService
    {
        bool IsAdmin(string username, string password);
    }
}
