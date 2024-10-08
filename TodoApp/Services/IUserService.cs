﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Services
{
    public interface IUserService
    {
        Task<int> AddUserAsync(User user);
        Task<User> GetUserAsync(string username, string password);
        Task<int> UpdateUserAsync(User user);
        Task<int> DeleteUserAsync(int userId);

        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<User> AuthenticateUserAsync(string username, string password);

        void Logout();
        bool IsUserAuthenticated { get; }
    }
}
