using Microsoft.Toolkit.Uwp.Notifications;
using Mopups.Services;
using Plugin.LocalNotification;
using System;

using TodoApp.Models;
using TodoApp.PopupPages;
using TodoApp.ViewModels;


namespace TodoApp.Services
{
    public class ReminderService:IReminderService
    {
        private readonly Dictionary<int, Reminder> _reminderStorage = new Dictionary<int, Reminder>();

        public void SetReminder(Reminder reminder)
        {
            try
            {
                _reminderStorage[reminder.TaskId] = reminder;


                var notification = new NotificationRequest
                {
                    NotificationId = reminder.TaskId,
                    Title = reminder.TaskName,
                    Description = $"Reminder for your task. Time: {reminder.ReminderDate:hh:mm tt}",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = reminder.ReminderDate,
                        RepeatType = reminder.IsDailyReminder ? NotificationRepeat.Daily : NotificationRepeat.No
                    },
                    Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions
                    {
                        LaunchAppWhenTapped = true,
                    }


                };

                LocalNotificationCenter.Current.Show(notification);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public Reminder GetReminderByTaskId(int taskId)
        {
            if (_reminderStorage.ContainsKey(taskId))
            {
                return _reminderStorage[taskId];
            }
            return null;
        }

        public void ShowReminderPopup(ReminderPopupViewModel viewModel)
        {
            var popup = new RemainderPopup(viewModel);
            MopupService.Instance.PushAsync(popup);
        }

        public void CancelReminder(int taskId)
        {
            LocalNotificationCenter.Current.Cancel(taskId);
            _reminderStorage.Remove(taskId);
        }
        public void DeleteReminderDetails(int taskId)
        {
            if (_reminderStorage.ContainsKey(taskId))
            {
                _reminderStorage.Remove(taskId);
            }
        }

    }

}
