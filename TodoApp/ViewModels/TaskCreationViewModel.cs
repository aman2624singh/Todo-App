using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.ViewModels
{
    public partial class TaskCreationViewModel:ObservableObject
    {
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly ITaskService _taskService;
        private readonly IUserSessionService _userSessionService;
        private readonly IPhotoService _photoService;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private DateTime dueDate = DateTime.Today;

        [ObservableProperty]
        private bool done;

        [ObservableProperty]
        private bool isPinned;

        [ObservableProperty]
        private string _notes;


        [ObservableProperty]
        private byte[] _attachment;

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
            var newTask = new TaskItem
            {
                Title = this.Title,
                DueDate = this.DueDate,
                Done = this.Done,
                IsPinned = this.IsPinned,
                Notes = this.Notes,
                Attachment = this.Attachment,
                UserId = CurrentuserId, 
            };

            int taskId = await _taskService.AddTaskAsync(newTask);
            currentTaskItemId = taskId;
            await SavePhotosToDatabase(taskId);
            await Application.Current.MainPage.DisplayAlert("Success", "Task and attachments saved!", "OK");
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
            string takePhotoOption = "Take Photo";
            string uploadPhotoOption = "Upload Attachment";
            var action = await Application.Current.MainPage.DisplayActionSheet(
                null,
                "Cancel",
                null,
                takePhotoOption,
                uploadPhotoOption
            );

            if (!string.IsNullOrEmpty(action))
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
                if (photo != null)
                {
                    var filePath = await SaveFile(photo);
                    PhotoFilePaths.Add(filePath); 
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task UploadPhoto()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Select an image"
                });

                if (result != null)
                {
                    var filePath = result.FullPath;
                    PhotoFilePaths.Add(filePath); 
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
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
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task OpenMenu()
        {

        }
    }
}
