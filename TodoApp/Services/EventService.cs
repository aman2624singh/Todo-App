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

        public async Task<int> AddEventAsync(Event eventItem)
        {
            return await _database.InsertAsync(eventItem);
        }

        public async Task<int> UpdateEventAsync(Event eventItem)
        {
            return await _database.UpdateAsync(eventItem);
        }

        public async Task<int> DeleteEventAsync(Event eventItem)
        {
            return await _database.DeleteAsync(eventItem);
        }

        public async Task<List<Event>> GetEventsByTaskAsync(int taskId)
        {
            return await _database.Table<Event>()
                .Where(e => e.TaskItemId == taskId)
                .ToListAsync();
        }

        public async Task<Event> GetEventAsync(int eventId)
        {
            return await _database.Table<Event>()
                .Where(e => e.Id == eventId)
                .FirstOrDefaultAsync();
        }
    }
}
