using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Services
{
    public interface ITaskService
    {
        Task<int> AddTaskAsync(TaskItem task);
        Task<int> UpdateTaskAsync(TaskItem task);
        Task<int> DeleteTaskAsync(TaskItem task);
        Task<List<TaskItem>> GetUserTasksAsync(int userId);
        Task<TaskItem> GetTaskAsync(int taskId);
    }
}
