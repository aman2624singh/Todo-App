using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
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

        public ObservableCollection<TaskItem> UserTasks { get; set; } = new ObservableCollection<TaskItem>();
        private ObservableCollection<string> PhotoFilePaths { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<Photo> AttachedPhotos { get; set; } = new ObservableCollection<Photo>();

        public TaskCreationViewModel(IUserService userService, INavigationService navigationService, IPhotoService photoService, ITaskService taskService, IUserSessionService userSessionService)
        {
            _userService = userService;
            _navigationService = navigationService;
            _taskService = taskService;
            _userSessionService = userSessionService;
            _photoService = photoService;
            CurrentuserId = _userSessionService.GetUserId();
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
                    Attachment = this.Attachment,
                    UserId = CurrentuserId,
                    HasAttachment = this.Attachment != null && this.Attachment.Length > 0
                };

                int taskId = await _taskService.AddTaskAsync(newTask);
                CurrentTaskItemId = taskId;
                await SavePhotosToDatabase(taskId);

                await Application.Current.MainPage.DisplayAlert(AppstringResources.Success, AppstringResources.Taskcreated, AppstringResources.OK);
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
                    existingTask.Attachment = this.Attachment;
                    existingTask.HasAttachment = this.Attachment != null && this.Attachment.Length > 0;

                    await _taskService.UpdateTaskAsync(existingTask);
                    await SavePhotosToDatabase(CurrentTaskItemId);

                    await Application.Current.MainPage.DisplayAlert(AppstringResources.Success, AppstringResources.Taskupdated, AppstringResources.OK);
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
                    TaskItemId = taskId 
                };

                await _photoService.AddPhotoAsync(photo);
            }
            PhotoFilePaths.Clear();
        }

        [RelayCommand]
        private async Task OnGalleryClicked()
        {
            string takePhotoOption = AppstringResources.TakePhoto;
            string uploadPhotoOption = Uploadattachmnet;
            var action = await Application.Current.MainPage.DisplayActionSheet(
                null,
                "Cancel",//add to string resource
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
                    PhotoFilePaths.Add(filePath); 
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
                    PickerTitle = Selectimage
                });

                if (result is not null)
                {
                    var filePath = result.FullPath;
                    PhotoFilePaths.Add(filePath); 
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
            try
            {
                await _photoService.DeletePhotoAsync(photo);
                AttachedPhotos.Remove(photo);
                if (File.Exists(photo.FilePath))
                {
                    File.Delete(photo.FilePath);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(AppstringResources.Error, ex.Message, AppstringResources.OK);
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

            if (query.ContainsKey("Attachments"))
            {
                var attachmentsJson = query["Attachments"]?.ToString();
                if (!string.IsNullOrEmpty(attachmentsJson))
                {
                    AttachedPhotos = JsonConvert.DeserializeObject<ObservableCollection<Photo>>(attachmentsJson);
                }
            }

             
        }
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly ITaskService _taskService;
        private readonly IUserSessionService _userSessionService;
        private readonly IPhotoService _photoService;

        private string Uploadattachmnet = "Upload Attachment";
        private string Selectimage = "Select an image";//put this is appstring resources(both of them)
    }
}
