using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Services
{
    public interface IEventService
    {
        Task<int> AddEventAsync(Event eventItem);
        Task<int> UpdateEventAsync(Event eventItem);
        Task<int> DeleteEventAsync(Event eventItem);
        Task<List<Event>> GetEventsByTaskAsync(int taskId);
        Task<Event> GetEventAsync(int eventId);
    }
}
