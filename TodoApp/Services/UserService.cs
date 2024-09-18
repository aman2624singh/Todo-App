using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class UserService : IUserService
    {
        private readonly SQLiteAsyncConnection _database;
        private User _currentUser;

        public UserService()
        {
            _database = DatabaseService.Database;
        }

        public async Task<int> AddUserAsync(User user)
        {
            return await _database.InsertAsync(user);
        }

        public async Task<User> GetUserAsync(string username, string passowrd)
        {
            return await _database.Table<User>()
                .Where(u => u.UserName == username && u.Password == passowrd)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateUserAsync(User user)
        {
            return await _database.UpdateAsync(user);
        }

        public async Task<int> DeleteUserAsync(User user)
        {
            return await _database.DeleteAsync(user);
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            var user = await _database.Table<User>()
                .Where(u => u.UserName == username && u.Password == password)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                _currentUser = user;
            }

            return user;
        }

        public void Logout()
        {
            _currentUser = null;

        }

        public bool IsUserAuthenticated => _currentUser != null;
    }
}
