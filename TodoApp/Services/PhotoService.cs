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
    public class PhotoService : IPhotoService
    {
        private readonly SQLiteAsyncConnection _database;

        public PhotoService()
        {
            _database = DatabaseService.Database;
        }

        public async Task<int> AddPhotoAsync(Photo photo)
        {
            return await _database.InsertAsync(photo);
        }

        public async Task<int> DeletePhotoAsync(Photo photo)
        {
            return await _database.DeleteAsync(photo);
        }

        public async Task<List<Photo>> GetPhotosByTaskAsync(int taskId)
        {
            return await _database.Table<Photo>()
                .Where(p => p.TaskItemId == taskId)
                .ToListAsync();
        }

        public async Task<Photo> GetPhotoAsync(int photoId)
        {
            return await _database.Table<Photo>()
                .Where(p => p.Id == photoId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Photo>> GetPhotosByTaskIdAsync(int taskId)
        {
            return await _database.Table<Photo>().Where(p => p.TaskItemId == taskId).ToListAsync();
        }
    }
}
