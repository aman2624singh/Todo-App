using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
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


        }

        [RelayCommand]
        private void SaveReminder(TaskItem task)
        {
            if (IsReminderEnabled)
            {
                var reminderDate = SelectedDate.Add(SelectedTime);

                if (!IsDailyReminderEnabled && SelectedDate == DateTime.Today && SelectedTime == DateTime.Now.TimeOfDay)
                {
                    reminderDate = DateTime.Now;
                }

                var reminder = new Reminder
                {
                    TaskId = CurrentTaskId,
                    TaskName = CurrentTaskName,
                    ReminderDate = reminderDate,
                    IsDailyReminder = IsDailyReminderEnabled
                };

                _reminderService.SetReminder(reminder);
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
            if (MopupService.Instance.PopupStack.Count > 0)
            {
                MopupService.Instance.PopAsync();
            }
        }
    }
}
