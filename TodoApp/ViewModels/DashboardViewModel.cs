using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using The49.Maui.BottomSheet;
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
            IsSidePanelVisible = !IsSidePanelVisible;
        }

        [ObservableProperty]
        private bool isPriority;

        [ObservableProperty]
        private bool isDate;

        [ObservableProperty]
        private bool isDone;

        [ObservableProperty]
        private bool isCancel;

        [ObservableProperty]
        private ObservableCollection<TaskItem> originalItems;

        [ObservableProperty]
        private SortOption selectedSortOption;

        [ObservableProperty]
        private ObservableCollection<TaskItem> items;

        [ObservableProperty]
        private ObservableCollection<TaskItem> pinnedItems;

        [ObservableProperty]
        private bool isempty;

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
        private string username;


        public List<SortOption> SortOptions { get; } = Enum.GetValues(typeof(SortOption)).Cast<SortOption>().ToList();


        public DashboardViewModel(IUserService userService, INavigationService navigationService,IReminderService reminderService, IEventService eventService, ITaskService taskService, IUserSessionService userSessionService)
        {
            _userService = userService;
            _navigationService = navigationService;
            _taskService = taskService;
            _userSessionService = userSessionService;
            _eventService = eventService;
            _reminderService= reminderService;
            string name= userSessionService.GetUserName();
            Username = $"Hi!! {name}";
        }

        public async void Initialize()
        {
           await LoadItems();
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

                filteredItems.ToList().ForEach(task => Items.Add(task));
                filteredPinnedItems.ToList().ForEach(task => PinnedItems.Add(task)); 

                IsListEmpty = !Items.Any();
                IsPinnedListEmpty = !PinnedItems.Any();
            }
        }

        [RelayCommand]
        private async Task TaskTapped(TaskItem task)
        {
            if (task is null) return;

            await Shell.Current.GoToAsync($"TaskcreationPage", new Dictionary<string, object>
    {
        { "TaskId", task.Id.ToString() },
        { "TaskName", task.Title },
        { "Priority", task.IsPriority },
        { "Done", task.Done },
        { "TaskDescription", task.Notes },
        { "DueDate", task.DueDate.ToString("o") }
    });
        }


        [RelayCommand]
        private async Task ReminderTask(TaskItem task)
        {
            if (task is null)
                return;

                var reminderPopupViewModel = new ReminderPopupViewModel(_reminderService)
                {
                    CurrentTaskId = task.Id,
                    CurrentTaskName = task.Title
                };
                _reminderService.ShowReminderPopup(reminderPopupViewModel);
        }


        [RelayCommand]
        private async Task ShowEventPopup( TaskItem task)
        {
            try
            {
                var eventViewModel = new EventBottomSheetViewModel(_eventService);
                var bottomSheet = new EventBottomSheet(eventViewModel);
                await eventViewModel.LoadEventsAsync(task.Id);
                await bottomSheet.ShowAsync();
            }
            catch (Exception ex)
            {

               
            }
        }


        [RelayCommand]
        private async Task DeleteTask(TaskItem task)
        {
            if (task is null)
                return;

            bool userConfirmed = await App.Current.MainPage.DisplayAlert(
                AppstringResources.Deletetask,
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
               // await App.Current.MainPage.DisplayAlert(AppstringResources.Error, "Failed to update task pin status: " + ex.Message, AppstringResources.OK);
            }

            IsPriority = PinnedItems.Any();
        }

        [RelayCommand]
        private void ApplySort(BottomSheet bottomsheet)
        {
            if (OriginalItems == null)
            {
                OriginalItems = new ObservableCollection<TaskItem>(Items);
            }
            switch (SelectedSortOption)
                {
                    case SortOption.Priority:
                        Items = new ObservableCollection<TaskItem>(items.OrderByDescending(item => item.IsPriority));
                        break;
                    case SortOption.Done:
                        Items = new ObservableCollection<TaskItem>(items.Where(item => item.Done).OrderByDescending(item => item.Done));
                        break;
                    case SortOption.Date:
                        Items = new ObservableCollection<TaskItem>(items.OrderBy(item => item.DueDate));
                        break;
                    case SortOption.None:
                        Items = new ObservableCollection<TaskItem>(originalItems);
                        break;
                default:
                        break;
                }
            bottomsheet.DismissAsync();
        }

        [RelayCommand]
        private async Task Showsortevent()
        {
            var bottomSheet = new SortSheet
            {
                BindingContext = this 
            };

            await bottomSheet.ShowAsync();
        }

        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly ITaskService _taskService;
        private readonly IUserSessionService _userSessionService;
        private readonly IEventService _eventService;
        private readonly IReminderService _reminderService;
    }
}
