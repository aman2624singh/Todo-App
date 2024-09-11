
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;
using TodoApp.Models;
using TodoApp.Resources.Strings;
using TodoApp.Services;

namespace TodoApp.ViewModels
{
    public  partial class EventPopupViewModel : ObservableObject
    {

        private readonly IEventService _eventService;


        [ObservableProperty]
        private string eventName;

        [ObservableProperty]
        private string eventDescription;

        [ObservableProperty]
        private DateTime selectedDate;

        [ObservableProperty]
        public ObservableCollection<TaskEvent> taskEvents;

        [ObservableProperty]
        private int taskItemId;

        [ObservableProperty]
        private TaskEvent selectedEvent;


        public EventPopupViewModel(IEventService eventService) 
        {
            _eventService = eventService;
        }

        public void Intialize(int taskItemId)
        {
            TaskItemId = taskItemId;
            SelectedDate = DateTime.Now;
            TaskEvents = new ObservableCollection<TaskEvent>();
            LoadEvents();
        }

        private async Task LoadEvents()
        {
            var events = await _eventService.GetEventsByTaskAsync(TaskItemId);
            TaskEvents.Clear();
            events.ToList().ForEach(taskEvent => TaskEvents.Add(taskEvent));
        }
        [RelayCommand]
        public async Task SaveEvent()
        {
            if (!string.IsNullOrWhiteSpace(EventName) && !string.IsNullOrWhiteSpace(EventDescription))
            {
                if (SelectedEvent is null)
                {
                    var newEvent = new TaskEvent
                    {
                        Eventname = EventName,
                        EventDate = SelectedDate,
                        EventDescription = EventDescription,
                        TaskItemId = TaskItemId
                    };

                    await _eventService.AddEventAsync(newEvent); 
                    TaskEvents.Add(newEvent); 
                }
                else
                {
                    SelectedEvent.Eventname = EventName;
                    SelectedEvent.EventDate = SelectedDate;
                    SelectedEvent.EventDescription = EventDescription;

                    await _eventService.UpdateEventAsync(SelectedEvent); 
                    var index = TaskEvents.IndexOf(SelectedEvent);
                    TaskEvents[index] = SelectedEvent; 
                }
                EventName = string.Empty;
                EventDescription = string.Empty;
                SelectedEvent = null;
            }
            else
            {
                
            }
        }

        [RelayCommand]
        public void EditEvent()
        {
            var selectedEvents = TaskEvents.Where(e => e.IsSelected).ToList();

            foreach (var taskEvent in selectedEvents)
            {
                if (taskEvent != null)
                {
                    EventName = taskEvent.Eventname;
                    SelectedDate = taskEvent.EventDate;
                    EventDescription = taskEvent.EventDescription;
                }
            }
        }

        [RelayCommand]
        public async Task DeleteEvent()
        {

            var selectedEvents = TaskEvents.Where(e => e.IsSelected).ToList();

            foreach (var taskEvent in selectedEvents)
            {
                if (taskEvent != null)
                {
                    await _eventService.DeleteEventAsync(taskEvent);
                    TaskEvents.Remove(taskEvent);
                }
            }

        }

        [RelayCommand]
        public void SetReminder(TaskEvent taskEvent)
        {
            
        }

        [RelayCommand]
        private void ClosePopup()
        {
            MopupService.Instance.PopAsync();
        }
    }

}
