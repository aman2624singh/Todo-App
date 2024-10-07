using SQLite;
using TodoApp.Data;
using User = TodoApp.Models.User;

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

        public async Task<int> DeleteUserAsync(int userId)
        {
            var user = await _database.Table<User>().Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user != null)
            {
                return await _database.DeleteAsync(user);
            }
            return 0;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _database.Table<User>().ToListAsync();
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

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _database.Table<User>()
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public void Logout()
        {
            _currentUser = null;

        }

        public bool IsUserAuthenticated => _currentUser != null;
    }
}
