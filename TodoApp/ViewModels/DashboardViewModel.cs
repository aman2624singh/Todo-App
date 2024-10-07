
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
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

        [ObservableProperty]

        private bool isAdmin;

        [ObservableProperty]
        private DateTime selectedDate;

        [ObservableProperty]
        private TimeSpan selectedTime;

        [ObservableProperty]
        private bool isDailyReminderEnabled;

        [ObservableProperty]
        private bool isReminderEnabled;

        [ObservableProperty]
        private DateTime eventDate = DateTime.Now;

        [ObservableProperty]
        private bool hasAttachments;

        [ObservableProperty]
        private int currentuserId;

        [ObservableProperty]
        private DateTime dueDate = DateTime.Today;

        [ObservableProperty]
        private bool done;

        [ObservableProperty]
        private bool isPriority;

        [ObservableProperty]
        private string notes;

        [ObservableProperty]
        private bool hasEvents;

        [ObservableProperty]
        private byte[] attachment;

        [ObservableProperty]
        private string currentTaskName;

        [ObservableProperty]
        private string taskTitle;

        [ObservableProperty]
        private bool onsidepandelVisible=true;

        [ObservableProperty]
        private bool isUserOptionsVisible;

        [ObservableProperty]
        private string eventName;

        [ObservableProperty]
        public int currentTaskItemId;

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

        public ObservableCollection<TaskEvent> Events { get; } = new ObservableCollection<TaskEvent>();

        private ObservableCollection<string> PhotoFilePaths { get; set; } = new ObservableCollection<string>();

        [ObservableProperty]
        private ObservableCollection<Photo> attachedPhotos = new ObservableCollection<Photo>();


        public DashboardViewModel(IUserService userService, ILogger<DashboardViewModel> logger, IPhotoService photoService, INavigationService navigationService,IReminderService reminderService, IEventService eventService, ITaskService taskService, IUserSessionService userSessionService)
        {
            _logger = logger;
            _userService = userService;
            _navigationService = navigationService;
            _taskService = taskService;
            _userSessionService = userSessionService;
            _eventService = eventService;
            _reminderService= reminderService;
            _photoService = photoService;
            string name= userSessionService.GetUserName();
            Username = $"Hi!! {name}";
        }

        public async void Initialize()
        {
           await LoadItems();
            CurrentuserId = _userSessionService.GetUserId();
            IsAdmin = _userSessionService.IsAdmin();
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
                _logger.LogError(ex, string.Empty);
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
            if(DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
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
            else if(DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                ToggleSidePanel(task);
                CurrentTaskItemId = task.Id;

            }                  
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
            reminderPopupViewModel.LoadReminder();

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

                _logger.LogError(ex, string.Empty);
            }
        }


        [RelayCommand]
        private async Task DeleteTask(TaskItem task)
        {
            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
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

            else if(DeviceInfo.Current.Platform==DevicePlatform.WinUI)
            {
                bool userConfirmed = await App.Current.MainPage.DisplayAlert(
                   AppstringResources.Deletetask,
                   $"Are you sure you want to delete the task: {CurrentTaskName}?",
                   AppstringResources.Yes,
                   AppstringResources.No
               );

                if (userConfirmed)
                {
                    try
                    {
                        var tasked = await _taskService.GetTaskByIdAsync(CurrentTaskItemId);
                        await _taskService.DeleteTaskAsync(tasked);
                        Items.Remove(tasked);
                        Items=new ObservableCollection<TaskItem>(Items);
                        await LoadItems();
                        IsSidePanelVisible = false;

                        System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();
                        var toast = Toast.Make($"{CurrentTaskName} Deleted 🗑", ToastDuration.Short, 16);
                        await toast.Show(cancellationTokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, string.Empty);
                        await App.Current.MainPage.DisplayAlert(AppstringResources.Error, ex.ToString(), AppstringResources.OK);
                    }
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
                await Toast.Make($"Task '{task.Title}' removed from priority.",ToastDuration.Short).Show();
                PinnedItems.Remove(task);
            }

            try
            {
                await _taskService.UpdateTaskAsync(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
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
                    Items = new ObservableCollection<TaskItem>(items.Where(item => item.IsPriority).OrderByDescending(item => item.IsPriority));
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

        public void ApplySortwindows(SortOption selectedSortOption)
        {
            if (OriginalItems is null)
            {
                OriginalItems = new ObservableCollection<TaskItem>(Items);
            }

            switch (selectedSortOption)
            {
                case SortOption.Priority:
                    Items = new ObservableCollection<TaskItem>(items.Where(item => item.IsPriority).OrderByDescending(item => item.IsPriority));
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

        [RelayCommand]
        private void ShowUserOptions()
        {
            IsUserOptionsVisible = !IsUserOptionsVisible;
        }

        [RelayCommand]
        private async Task DesktopTask()
        {
            var userId = _userSessionService.GetUserId();
            if (string.IsNullOrWhiteSpace(this.TaskTitle))
            {
                await Application.Current.MainPage.DisplayAlert(AppstringResources.Error, AppstringResources.TitleandDescriptionerror, AppstringResources.OK);
                return;
            }
            var newTask = new TaskItem
                {
                    Title = this.TaskTitle,
                    UserId= userId

                };

                await _taskService.AddTaskAsync(newTask);
                 Items.Add(newTask);
                 TaskTitle = string.Empty;
                await Toast.Make(AppstringResources.Taskcreated).Show();
        }

        [RelayCommand]
        private async Task Savetaskclicked()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentTaskName) || string.IsNullOrWhiteSpace(Notes))
                {
                    await Application.Current.MainPage.DisplayAlert(AppstringResources.Error, AppstringResources.TitleandDescriptionerror, AppstringResources.OK);
                    return;
                }

                if (CurrentTaskItemId == 0) 
                {
                    var newTask = new TaskItem
                    {
                        Title = CurrentTaskName,
                        DueDate = DueDate,
                        Done = Done,
                        IsPriority = IsPriority,
                        Notes = Notes,
                        UserId = CurrentuserId,
                        HasAttachment = Attachment != null && Attachment.Length > 0
                    };

                    int taskId = await _taskService.AddTaskAsync(newTask);
                    CurrentTaskItemId = taskId;
                    await SavePhotosToDatabase(taskId);

                    if (IsReminderEnabled)
                    {
                        var reminderDate = SelectedDate.Date + SelectedTime;
                        if (!IsDailyReminderEnabled && SelectedDate == DateTime.Today && SelectedTime == DateTime.Now.TimeOfDay)
                        {
                            reminderDate = DateTime.Now;
                        }

                        var reminder = new Reminder
                        {
                            TaskId = CurrentTaskItemId,
                            TaskName = CurrentTaskName,
                            ReminderDate = reminderDate,
                            IsDailyReminder = IsDailyReminderEnabled
                        };

                        _reminderService.SetReminder(reminder);
                    }
                    else
                    {
                        _reminderService.CancelReminder(CurrentTaskItemId);
                        _reminderService.DeleteReminderDetails(CurrentTaskItemId);
                    }

                    await Toast.Make(AppstringResources.Taskcreated).Show();
                }
                else
                {
                    var existingTask = await _taskService.GetTaskByIdAsync(CurrentTaskItemId);
                    if (existingTask is not null)
                    {
                        existingTask.Title = CurrentTaskName;
                        existingTask.DueDate = DueDate;
                        existingTask.Done = IsDone;
                        existingTask.IsPriority = IsPriority;
                        existingTask.Notes = Notes;
                        existingTask.HasAttachment = Attachment is not null && Attachment.Length > 0;

                        await _taskService.UpdateTaskAsync(existingTask);
                        await SavePhotosToDatabase(CurrentTaskItemId);

                        if (IsReminderEnabled)
                        {
                            var reminderDate = SelectedDate.Date + SelectedTime;
                            if (!IsDailyReminderEnabled && SelectedDate == DateTime.Today && SelectedTime == DateTime.Now.TimeOfDay)
                            {
                                reminderDate = DateTime.Now;
                            }
                            var reminder = new Reminder
                            {
                                TaskId = CurrentTaskItemId,
                                TaskName = CurrentTaskName,
                                ReminderDate = reminderDate,
                                IsDailyReminder = IsDailyReminderEnabled
                            };

                            _reminderService.SetReminder(reminder);
                        }
                        else
                        {
                            _reminderService.CancelReminder(CurrentTaskItemId);
                            _reminderService.DeleteReminderDetails(CurrentTaskItemId);
                        }

                        await Toast.Make(AppstringResources.Taskupdated).Show();
                    }
                }

                Items = new ObservableCollection<TaskItem>(Items);
                await LoadItems();
                IsSidePanelVisible = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
            }
        }


        [RelayCommand]
        private async Task AddEvent()
        {
            if (!string.IsNullOrWhiteSpace(EventName))
            {
                var newEvent = new TaskEvent
                {
                    EventName = EventName,
                    IsDone = false,
                    TaskItemId = CurrentTaskItemId,
                };

                await _eventService.AddEventAsync(newEvent);

                Events.Add(newEvent);

                EventName = string.Empty;
                EventDate = DateTime.Now;
                HasEvents = Events.Count > 0;

            }
        }

        public async Task LoadEventsAsync(int taskId)
        {
            CurrentTaskItemId = taskId;
            var events = await _eventService.GetEventsByTaskAsync(taskId);

            Events.Clear();
            foreach (var ev in events)
            {
                Events.Add(ev);
            }
            HasEvents = Events.Any();
        }

        [RelayCommand]
        private async Task ToggleSidePanel(TaskItem task)
        {
            IsSidePanelVisible = !IsSidePanelVisible;
            OnsidepandelVisible = !IsSidePanelVisible;

           await LoadTaskDetails(task.Id);
           await LoadEventsAsync(task.Id);
           await  LoadTaskAndPhotos(task.Id);
            LoadReminder();
        }

        private async Task LoadTaskDetails(int taskId)
        {
            var task = await _taskService.GetTaskByIdAsync(taskId);
            if (task is not null)
            {
                CurrentTaskName = task.Title;
                DueDate = task.DueDate;
                IsDone = task.Done;
                IsPriority = task.IsPriority;
                Notes = task.Notes;
                HasAttachments = task.HasAttachment;
            }
        }

        private async Task SavePhotosToDatabase(int taskId)
        {
            foreach (var filePath in PhotoFilePaths)
            {
                var photo = new Photo
                {
                    FilePath = filePath,
                    FileName = Path.GetFileName(filePath),
                    TaskItemId = taskId
                };

                await _photoService.AddPhotoAsync(photo);
            }
            PhotoFilePaths.Clear();
        }

        [RelayCommand]
        private async Task GalleryClicked()
        {
            string takePhotoOption = AppstringResources.TakePhoto;
            string uploadPhotoOption = AppstringResources.UploadAttachmnet;
            var action = await Application.Current.MainPage.DisplayActionSheet(
                null,
                AppstringResources.Cancel,
                null,
                takePhotoOption,
                uploadPhotoOption
            );

            if (!string.IsNullOrWhiteSpace(action))
            {
                if (action == takePhotoOption)
                {
                    await TakePhoto();
                }
                else if (action == uploadPhotoOption)
                {
                    await UploadPhoto();
                }
            }
        }

        private async Task TakePhoto()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo is not null)
                {
                    var filePath = await SaveFile(photo);

                    if (string.IsNullOrEmpty(filePath))
                    {
                        await Application.Current.MainPage.DisplayAlert(AppstringResources.Error, AppstringResources.Photopath, AppstringResources.OK);
                        return;
                    }

                    if (PhotoFilePaths is null)
                    {
                        PhotoFilePaths = new ObservableCollection<string>();
                    }

                    PhotoFilePaths.Add(filePath);

                    if (AttachedPhotos is null)
                    {
                        AttachedPhotos = new ObservableCollection<Photo>();
                    }

                    AttachedPhotos.Add(new Photo
                    {
                        FilePath = filePath,
                        FileName = photo.FileName,
                        TaskItemId = CurrentTaskItemId
                    });
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(AppstringResources.Error, ex.Message, AppstringResources.OK);
            }
        }

        private async Task UploadPhoto()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = AppstringResources.SelectImage
                });

                if (result is not null)
                {
                    var filePath = result.FullPath;

                    if (string.IsNullOrEmpty(filePath))
                    {
                        await Application.Current.MainPage.DisplayAlert(AppstringResources.Error, AppstringResources.Photopath, AppstringResources.OK);
                        return;
                    }

                    if (PhotoFilePaths is null)
                    {
                        PhotoFilePaths = new ObservableCollection<string>();
                    }
                    PhotoFilePaths.Add(filePath);

                    if (AttachedPhotos is null)
                    {
                        AttachedPhotos = new ObservableCollection<Photo>();
                    }

                    AttachedPhotos.Add(new Photo
                    {
                        FilePath = filePath,
                        FileName = result.FileName,
                        TaskItemId = CurrentTaskItemId
                    });
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(AppstringResources.Error, ex.Message, AppstringResources.OK);
            }
        }

        private async Task<string> SaveFile(FileResult photo)
        {
            var newFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFilePath))
            {
                await stream.CopyToAsync(newStream);
            }
            return newFilePath;
        }


        [RelayCommand]
        private async Task DeletePhoto(Photo photo)
        {
            bool isConfirmed = await App.Current.MainPage.DisplayAlert(
              AppstringResources.Deletetask,
              $"Are you sure you want to delete the task: {photo.FileName}?",
              AppstringResources.Yes,
              AppstringResources.No
          );
            if (isConfirmed)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(photo);
                    AttachedPhotos.Remove(photo);
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert(AppstringResources.Error, $"Failed to delete photo: {ex.Message}", AppstringResources.OK);
                }
            }
        }

        private async Task LoadTaskAndPhotos(int taskId)
        {
            var existingTask = await _taskService.GetTaskByIdAsync(taskId);
            if (existingTask is not null)
            {
                await LoadPhotosFromDatabase(taskId);
            }
        }
        private async Task LoadPhotosFromDatabase(int taskId)
        {
            var photos = await _photoService.GetPhotosByTaskIdAsync(taskId);
            AttachedPhotos.Clear();
            foreach (var photo in photos)
            {
                AttachedPhotos.Add(photo);
            }
        }

        public void LoadReminder()
        {
            var existingReminder = _reminderService.GetReminderByTaskId(CurrentTaskItemId);

            if (existingReminder is not null)
            {
                SelectedDate = existingReminder.ReminderDate.Date;
                SelectedTime = existingReminder.ReminderDate.TimeOfDay;
                IsDailyReminderEnabled = existingReminder.IsDailyReminder;
                IsReminderEnabled = true;
            }
            else
            {
                SelectedDate = DateTime.Today;
                SelectedTime = DateTime.Now.TimeOfDay;
                IsDailyReminderEnabled = false;
                IsReminderEnabled = false;
            }

        }

        [RelayCommand]
        public async Task DoneEventAsync(TaskEvent eventItem)
        {
            if (eventItem is not null)
            {
                eventItem.IsDone = !eventItem.IsDone;

                await _eventService.UpdateEventAsync(eventItem);

                var statusMessage = eventItem.IsDone ? AppstringResources.Markeddone : AppstringResources.Markednotdone;
                await Toast.Make(statusMessage, ToastDuration.Short).Show();
            }
        }

        [RelayCommand]
        public async Task DeleteEventAsync(TaskEvent eventItem)
        {
            if (eventItem is not null)
            {
                await _eventService.DeleteEventAsync(eventItem);
                Events.Remove(eventItem);
            }
        }

        [RelayCommand]
        public async Task Back()
        {
            try
            {
                await Shell.Current.GoToAsync("//AdminDashBoard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
            }
        }

        [RelayCommand]
        private async Task ViewPhoto(Photo photo)
        {
            if (photo is null)
                return;

            try
            {
                if (!File.Exists(photo.FilePath))
                {
                    await Application.Current.MainPage.DisplayAlert(
                        AppstringResources.Error,
                        AppstringResources.Photopath,
                        AppstringResources.OK
                    );
                    return;
                }


#if ANDROID || IOS

                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(photo.FilePath)
                });
#elif WINDOWS
                // For Windows, use Process.Start with shell execution
                Process.Start(new ProcessStartInfo
        {
            FileName = photo.FilePath,
            UseShellExecute = true
        });
#else
        await Application.Current.MainPage.DisplayAlert(
            AppstringResources.Error, 
            "File opening is not supported on this platform.", 
            AppstringResources.OK
        );
#endif
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    AppstringResources.Error,
                    ex.Message,
                    AppstringResources.OK
                );
            }
        }

        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly ITaskService _taskService;
        private readonly IUserSessionService _userSessionService;
        private readonly IEventService _eventService;
        private readonly IReminderService _reminderService;
        private readonly IPhotoService _photoService;
        private readonly ILogger<DashboardViewModel> _logger;

        private bool _isSidePanelVisible;
        private GridLength _sidePanelWidth;

    }
}
