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
        Task<int> AddEventAsync(TaskEvent eventItem);
        Task<int> UpdateEventAsync(TaskEvent eventItem);
        Task<int> DeleteEventAsync(TaskEvent eventItem);
        Task<List<TaskEvent>> GetEventsByTaskAsync(int taskId);
        Task<TaskEvent> GetEventAsync(int eventId);
    }
}
