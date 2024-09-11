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
    public class EventService : IEventService
    {
        private readonly SQLiteAsyncConnection _database;

        public EventService()
        {
            _database = DatabaseService.Database;
        }

        public async Task<int> AddEventAsync(TaskEvent eventItem)
        {
            return await _database.InsertAsync(eventItem);
        }

        public async Task<int> UpdateEventAsync(TaskEvent eventItem)
        {
            return await _database.UpdateAsync(eventItem);
        }

        public async Task<int> DeleteEventAsync(TaskEvent eventItem)
        {
            return await _database.DeleteAsync(eventItem);
        }

        public async Task<List<TaskEvent>> GetEventsByTaskAsync(int taskId)
        {
            return await _database.Table<TaskEvent>()
                .Where(e => e.TaskItemId == taskId)
                .ToListAsync();
        }

        public async Task<TaskEvent> GetEventAsync(int eventId)
        {
            return await _database.Table<TaskEvent>()
                .Where(e => e.Id == eventId)
                .FirstOrDefaultAsync();
        }
    }
}
