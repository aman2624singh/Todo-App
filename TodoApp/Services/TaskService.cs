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
    public class TaskService : ITaskService
    {
        private readonly SQLiteAsyncConnection _database;

        public TaskService()
        {
            _database = DatabaseService.Database;
        }

        public async Task<int> AddTaskAsync(TaskItem task)
        {
            await _database.InsertAsync(task);
            return task.Id;
        }

        public async Task<int> UpdateTaskAsync(TaskItem task)
        {
            return await _database.UpdateAsync(task);
        }

        public async Task<int> DeleteTaskAsync(TaskItem task)
        {
            return await _database.DeleteAsync(task);
        }

        public async Task<List<TaskItem>> GetUserTasksAsync(int userId)
        {
            return await _database.Table<TaskItem>()
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<TaskItem> GetTaskAsync(int taskId)
        {
            return await _database.Table<TaskItem>()
                .Where(t => t.Id == taskId)
                .FirstOrDefaultAsync();
        }
    }
}
