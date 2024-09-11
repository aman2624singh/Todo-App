using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models;
using TodoApp.PopupPages;
using TodoApp.Resources.Strings;
using TodoApp.Services;
using TodoApp.Views;


namespace TodoApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private bool _isSidePanelVisible;
        private GridLength _sidePanelWidth;
        public bool IsSidePanelVisible
        {
            get => _isSidePanelVisible;
            set
            {
                SetProperty(ref _isSidePanelVisible, value);
                SidePanelWidth = value ? new GridLength(350, GridUnitType.Absolute) : new GridLength(0, GridUnitType.Absolute);
            }
        }

        public GridLength SidePanelWidth
        {
            get => _sidePanelWidth;
            set => SetProperty(ref _sidePanelWidth, value);
        }

        [RelayCommand]
        private void ToggleSidePanel()
        {
            // Toggle the visibility of the side panel
            IsSidePanelVisible = !IsSidePanelVisible;
        }

        [ObservableProperty]
        private ObservableCollection<TaskItem> items;

        [ObservableProperty]
        private ObservableCollection<TaskItem> pinnedItems;

        [ObservableProperty]
        private bool isListEmpty;

        [ObservableProperty]
        private bool isPinnedListEmpty;

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    SearchTasks();
                }
            }
        }

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool isempty;

        [ObservableProperty]
        private string username;


        [ObservableProperty]
        private bool isPriority;

        [ObservableProperty]
        private bool isPickerVisible;

        private SortOption _selectedSortOption;
        public SortOption SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (SetProperty(ref _selectedSortOption, value))
                {
                    SortTask();
                }
            }
        }


        public List<SortOption> SortOptions { get; } = Enum.GetValues(typeof(SortOption)).Cast<SortOption>().ToList();


        public DashboardViewModel(IUserService userService, INavigationService navigationService,IReminderService reminderService, IEventService eventService, ITaskService taskService, IUserSessionService userSessionService)
        {
            _userService = userService;
            _navigationService = navigationService;
            _taskService = taskService;
            _userSessionService = userSessionService;
            _eventService = eventService;
            _reminderService= reminderService;
            Username = userSessionService.GetUserName();
        }

        public async void Initialize()
        {
           await LoadItems();
            IsPickerVisible = false;
        }

        [RelayCommand]
        private async Task TaskAdded()
        {
            await _navigationService.NavigateWithoutRootAsync(nameof(TaskcreationPage));

        }

        private async Task LoadItems()
        {
            try
            {
                var userId = _userSessionService.GetUserId();
                var tasks = await _taskService.GetUserTasksAsync(userId);

                Items ??= new ObservableCollection<TaskItem>();
                PinnedItems ??= new ObservableCollection<TaskItem>();
                Items.Clear();
                PinnedItems.Clear();


                if (tasks is not null && tasks.Any())
                {
                    foreach (var task in tasks)
                    {
                        task.IsSelected = false;
                        Items.Add(task);
                        if (task.IsPriority)
                            PinnedItems.Add(task);
                    }
                    Isempty = false;
                    IsPriority = PinnedItems.Any();
                }
                else
                {
                    Isempty = true;
                    IsPriority = false;
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppstringResources.Error, ex.ToString(), AppstringResources.OK);
            }
        }


        private void SearchTasks()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Initialize();
            }
            else
            {
                var filteredItems = Items.Where(task => task.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                var filteredPinnedItems = PinnedItems.Where(task => task.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

                Items.Clear();
                PinnedItems.Clear();

                foreach (var task in filteredItems) Items.Add(task);
                foreach (var task in filteredPinnedItems) PinnedItems.Add(task);//can use linq query here 

                IsListEmpty = !Items.Any();
                IsPinnedListEmpty = !PinnedItems.Any();
            }
        }

        [RelayCommand]
        private async Task TaskTapped(TaskItem task)
        {
            if (task is null) return;

            var attachmentsJson = JsonConvert.SerializeObject(task.Attachment); 
            await Shell.Current.GoToAsync($"TaskcreationPage", new Dictionary<string, object>
    {
        { "TaskId", task.Id.ToString() },
        { "TaskName", task.Title },
        { "Priority", task.IsPriority },
        { "Done", task.Done },
        { "TaskDescription", task.Notes },
        { "DueDate", task.DueDate.ToString("o") }, 
        { "Attachments", attachmentsJson } 
    });
        }


        [RelayCommand]
        private void ShowPicker()
        {
            IsPickerVisible = !IsPickerVisible;
        }
        private void SortTask()
        {
            if (Items == null || !Items.Any()) return;

            IEnumerable<TaskItem> sortedTasks = Items;

            switch (SelectedSortOption)
            {
                case SortOption.Recent:
                    sortedTasks = Items.OrderByDescending(task => task.DueDate);
                    break;

                case SortOption.Done:
                    sortedTasks = Items.OrderBy(task => task.Done);
                    break;

                case SortOption.Pinned:
                    sortedTasks = Items.OrderByDescending(task => task.IsPriority);
                    break;
            }

            Items.Clear();
            foreach (var task in sortedTasks)
            {
                Items.Add(task);
            }
        }


        [RelayCommand]
        private async Task RefreshTasksAsync()
        {
            IsRefreshing = true;
            await LoadItems();
            IsRefreshing = false;
        }

        [RelayCommand]
        private async Task Reminder()
        {
            var selectedTasks = Items.Where(task => task.IsSelected).ToList();
            if (!selectedTasks.Any())
            {
                await App.Current.MainPage.DisplayAlert("No Task Selected", "Please select a task for the reminder.", "OK");
                return;
            }

            foreach (var task in selectedTasks)
            {
                var reminderPopupViewModel = new ReminderPopupViewModel(_reminderService)
                {
                    CurrentTaskId = task.Id,
                    CurrentTaskName = task.Title
                };
                _reminderService.ShowReminderPopup(reminderPopupViewModel);
            }
        }

        [RelayCommand]
        private async void ShowEventPopup( TaskItem task)
        {
            var eventPopupViewModel = new EventPopupViewModel(_eventService);
            var eventPopup = new EventPopup(eventPopupViewModel, task.Id);

            await MopupService.Instance.PushAsync(eventPopup);
        }

        [RelayCommand]
        private async Task DeleteTask(TaskItem task)
        {
            if (task is null)
                return;

            bool userConfirmed = await App.Current.MainPage.DisplayAlert(
                "Delete Task",
                $"Are you sure you want to delete the task: {task.Title}?",
                AppstringResources.Yes,
                AppstringResources.No
            );

            if (userConfirmed)
            {
                try
                {
                    await _taskService.DeleteTaskAsync(task);
                    Items.Remove(task);

                    if (task.IsPriority)
                        PinnedItems.Remove(task);

                    IsPriority = PinnedItems.Any();
                    Isempty = !Items.Any();

                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    var toast = Toast.Make($"{task.Title} Deleted 🗑", ToastDuration.Short, 16);
                    await toast.Show(cancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert(AppstringResources.Error, ex.ToString(), AppstringResources.OK);
                }
            }
        }

        [RelayCommand]
        private async Task TogglePinStatus(TaskItem task)
        {
            if (task is  null) return;

            task.IsPriority = !task.IsPriority;

            if (task.IsPriority)
            {
                if (!PinnedItems.Contains(task))
                    PinnedItems.Add(task);
            }
            else
            {
                PinnedItems.Remove(task);
            }

            try
            {
                await _taskService.UpdateTaskAsync(task);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Failed to update task pin status: " + ex.Message, "OK");
            }

            IsPriority = PinnedItems.Any();
        }

        [RelayCommand]
        private async Task PriorityList()
        {
            var priorityViewModel = new PriorityVIewModel();
            var priorityPopup = new Priority(priorityViewModel);

            await MopupService.Instance.PushAsync(priorityPopup);
        }

        private string Pinstatus = "Failed to update task pin status: ";
        private string Deleteall = "Delete All Tasks";
        private string Areusure = "Are you sure you want to delete ALL tasks?";
        private string AlltaskDeleted = "All Task(s) Deleted 🗑";
        private string SelectedTask = "Delete Selected Tasks";
        private string Allselctedtask = "Are you sure you want to delete the selected tasks?";
        private string Selectedtaskdeleted = "Selected Task(s) Deleted 🗑";
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly ITaskService _taskService;
        private readonly IUserSessionService _userSessionService;
        private readonly IEventService _eventService;
        private readonly IReminderService _reminderService;
    }
}
