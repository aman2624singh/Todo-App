using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TodoApp.Models;
using TodoApp.Resources.Strings;
using TodoApp.Services;
using TodoApp.Views;


namespace TodoApp.ViewModels
{
    public partial class TaskCreationViewModel:ObservableObject, IQueryAttributable
    {
       
        [ObservableProperty]
        private bool isRunning;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private DateTime dueDate = DateTime.Today;

        [ObservableProperty]
        private bool done;

        [ObservableProperty]
        private bool isPriority;

        [ObservableProperty]
        private string notes;


        [ObservableProperty]
        private byte[] attachment;

        [ObservableProperty]
        private int currentuserId;

        [ObservableProperty]
        public int currentTaskItemId;

        private ObservableCollection<string> PhotoFilePaths { get; set; } = new ObservableCollection<string>();

        [ObservableProperty]
        private ObservableCollection<Photo> attachedPhotos = new ObservableCollection<Photo>();

        public TaskCreationViewModel(IUserService userService, INavigationService navigationService, IPhotoService photoService, ITaskService taskService, IUserSessionService userSessionService)
        {
            _userService = userService;
            _navigationService = navigationService;
            _taskService = taskService;
            _userSessionService = userSessionService;
            _photoService = photoService;
            CurrentuserId = _userSessionService.GetUserId();
            
        }

        public void Initialize()
        {
            LoadTaskAndPhotos(currentTaskItemId);
        }

        private async Task LoadTaskAndPhotos(int taskId)
        {
            var existingTask = await _taskService.GetTaskByIdAsync(taskId);
            if (existingTask != null)
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

        [RelayCommand]
        private async Task SavedClicked()
        {
            if (string.IsNullOrWhiteSpace(this.Title) || string.IsNullOrWhiteSpace(this.Notes))
            {
                await Application.Current.MainPage.DisplayAlert(AppstringResources.Error, AppstringResources.TitleandDescriptionerror,AppstringResources.OK);
                return;
            }

            if (CurrentTaskItemId == 0) 
            {
                var newTask = new TaskItem
                {
                    Title = this.Title,
                    DueDate = this.DueDate,
                    Done = this.Done,
                    IsPriority = this.IsPriority,
                    Notes = this.Notes,
                    UserId = CurrentuserId,
                    HasAttachment = this.Attachment != null && this.Attachment.Length > 0
                };

                int taskId = await _taskService.AddTaskAsync(newTask);
                CurrentTaskItemId = taskId;
                await SavePhotosToDatabase(taskId);

                await Toast.Make(AppstringResources.Taskcreated).Show();
            }
            else 
            {
                var existingTask = await _taskService.GetTaskByIdAsync(CurrentTaskItemId);
                if (existingTask != null)
                {
                    existingTask.Title = this.Title;
                    existingTask.DueDate = this.DueDate;
                    existingTask.Done = this.Done;
                    existingTask.IsPriority = this.IsPriority;
                    existingTask.Notes = this.Notes;
                    existingTask.HasAttachment = this.Attachment != null && this.Attachment.Length > 0;

                    await _taskService.UpdateTaskAsync(existingTask);
                    await SavePhotosToDatabase(CurrentTaskItemId);

                    await Toast.Make(AppstringResources.Taskupdated).Show();
                }
            }

            await _navigationService.NavigateWithoutRootAsync(nameof(Dashboard));
        }

        private async Task SavePhotosToDatabase(int taskId)
        {
            foreach (var filePath in PhotoFilePaths)
            {
                var photo = new Photo
                {
                    FilePath = filePath,
                    FileName= Path.GetFileName(filePath),
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
                "Cancel",
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
                        await Application.Current.MainPage.DisplayAlert(AppstringResources.Error, AppstringResources.Photopath , AppstringResources.OK );
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
                        FileName=result.FileName,
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

        [RelayCommand]
        private async Task OpenMenu()
        {

        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("TaskId"))
            {
                CurrentTaskItemId = int.Parse(query["TaskId"].ToString());
            }

            if (query.ContainsKey("TaskName"))
            {
                Title = query["TaskName"]?.ToString();
            }

            if (query.ContainsKey("TaskDescription"))
            {
                Notes = query["TaskDescription"]?.ToString();
            }

            if (query.ContainsKey("DueDate"))
            {
                DueDate = DateTime.Parse(query["DueDate"].ToString());
            }

            if (query.ContainsKey("Done"))
            {
                Done = bool.Parse(query["Done"].ToString());
            }

            if (query.ContainsKey("Priority"))
            {
                IsPriority = bool.Parse(query["Priority"].ToString());
            }

        }

        [RelayCommand]
        private async Task ViewPhoto(Photo photo)
        {
            if (photo == null)
                return;
            try
            {
                if (!File.Exists(photo.FilePath))
                {
                    await Application.Current.MainPage.DisplayAlert(AppstringResources.Error, AppstringResources.Photopath, AppstringResources.OK);
                    return;
                }
                var fileUri = new Uri(photo.FilePath, UriKind.Absolute);
                await Launcher.OpenAsync(fileUri);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(AppstringResources.Error, ex.Message, AppstringResources.OK);
            }
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await _navigationService.NavigateWithoutRootAsync(nameof(Dashboard));
        }
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly ITaskService _taskService;
        private readonly IUserSessionService _userSessionService;
        private readonly IPhotoService _photoService;

    }
}
