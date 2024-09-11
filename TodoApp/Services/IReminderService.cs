using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models;
using TodoApp.ViewModels;

namespace TodoApp.Services
{
    public interface IReminderService
    {
        void SetReminder(Reminder reminder);
        void ShowReminderPopup(ReminderPopupViewModel viewModel);
        void CancelReminder(int taskId);
    }
}
