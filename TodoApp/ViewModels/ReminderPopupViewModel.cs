using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.ViewModels
{
    public partial class ReminderPopupViewModel:ObservableObject
    {
        private readonly IReminderService _reminderService;

        [ObservableProperty]
        private DateTime selectedDate;

        [ObservableProperty]
        private TimeSpan selectedTime;

        [ObservableProperty]
        private bool isDailyReminderEnabled;

        [ObservableProperty]
        private bool isReminderEnabled;

        [ObservableProperty]
        private int currentTaskId;

        [ObservableProperty]
        private string currentTaskName;

        public ReminderPopupViewModel(IReminderService reminderService)
        {
            _reminderService = reminderService;
            
        }

        public void Initialize()
        {
            SelectedDate = DateTime.Today;
            SelectedTime = DateTime.Now.TimeOfDay;
            IsDailyReminderEnabled = false;
            IsReminderEnabled = true;

        }

        [RelayCommand]
        private void SaveReminder(TaskItem task)
        {
            if (IsReminderEnabled)
            {
                _reminderService.SetReminder(new Reminder
                {
                    TaskId = CurrentTaskId,
                    TaskName= CurrentTaskName,
                    ReminderDate = SelectedDate.Add(SelectedTime),
                    IsDailyReminder = IsDailyReminderEnabled
                });
            }
            ClosePopup();
        }

        [RelayCommand]
        private void CancelReminder()
        {
            ClosePopup();
        }

        [RelayCommand]
        private void ClosePopup()
        {
            // Logic to close the popup
            // This can be a service that hides the popup
        }
    }
}
