using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The49.Maui.BottomSheet;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.ViewModels
{
    public partial class EventBottomSheetViewModel:ObservableObject
    {
        private readonly IEventService _eventService;

        [ObservableProperty]
        private string eventName;

        [ObservableProperty]
        private DateTime eventDate = DateTime.Now;

        public ObservableCollection<TaskEvent> Events { get; } = new ObservableCollection<TaskEvent>();

        [ObservableProperty]
        private int currentTaskId; 

        public EventBottomSheetViewModel(IEventService eventService)
        {
            _eventService = eventService;
        }


        public async Task LoadEventsAsync(int taskId)
        {
            CurrentTaskId = taskId;
            var events = await _eventService.GetEventsByTaskAsync(taskId);

            Events.Clear();
            foreach (var ev in events)
            {
                Events.Add(ev);
            }
        }

        [RelayCommand]
        private async Task AddEventAsync(BottomSheet bottomsheet)
        {
            if (!string.IsNullOrWhiteSpace(EventName))
            {
                var newEvent = new TaskEvent
                {
                    EventName = EventName,
                    IsDone=false,
                    TaskItemId = CurrentTaskId,
                };

                await _eventService.AddEventAsync(newEvent);

                Events.Add(newEvent);

                EventName = string.Empty;
                EventDate = DateTime.Now;

            }
            await bottomsheet.DismissAsync();
        }

        [RelayCommand]
        private async Task DoneEventAsync(TaskEvent eventItem)
        {
            if (eventItem != null)
            {
                eventItem.IsDone = !eventItem.IsDone;

                await _eventService.UpdateEventAsync(eventItem);

                var statusMessage = eventItem.IsDone ? "Marked as done!" : "Marked as not done!";
                await Toast.Make(statusMessage, ToastDuration.Short).Show();
            }
        }
    
        [RelayCommand]
        private async Task DeleteEventAsync(TaskEvent eventItem)
        {
            if (eventItem != null)
            {
                await _eventService.DeleteEventAsync(eventItem);
                Events.Remove(eventItem);
            }
        }
    }
}
