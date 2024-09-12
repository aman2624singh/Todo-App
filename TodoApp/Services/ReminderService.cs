using Mopups.Services;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models;
using TodoApp.PopupPages;
using TodoApp.ViewModels;

namespace TodoApp.Services
{
    public class ReminderService:IReminderService
    {
        public void SetReminder(Reminder reminder)
    {
        // Schedule the notification
        var notification = new NotificationRequest
        {
            NotificationId = reminder.TaskId, // Unique ID for each reminder
            Title = reminder.TaskName,
            Description = $"Reminder for your task. Time: {reminder.ReminderDate:hh:mm tt}",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = reminder.ReminderDate,
                RepeatType = reminder.IsDailyReminder ? NotificationRepeat.Daily : NotificationRepeat.No
            }
        };

        // Show the local notification
        LocalNotificationCenter.Current.Show(notification);
    }

        public void ShowReminderPopup(ReminderPopupViewModel viewModel)
        {
            // Create an instance of RemainderPopup and pass the ViewModel via the constructor
            var popup = new RemainderPopup(viewModel);

            // Show the popup using MopupService
            MopupService.Instance.PushAsync(popup);
        }

        public void CancelReminder(int taskId)
    {
        // Cancel the notification
        LocalNotificationCenter.Current.Cancel(taskId);
    }
}
}
