using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
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
using TodoApp.Views;

namespace TodoApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly ITaskService _taskService;
        private readonly IUserSessionService _userSessionService;

        [ObservableProperty]
        private ObservableCollection<TaskItem> items;

        [ObservableProperty]
        private ObservableCollection<TaskItem> pinnedItems;

        [ObservableProperty]
        private bool isListEmpty;

        [ObservableProperty]
        private bool isPinnedListEmpty;

        public DashboardViewModel(IUserService userService, INavigationService navigationService, ITaskService taskService, IUserSessionService userSessionService)
        {
            _userService = userService;
            _navigationService = navigationService;
            _taskService = taskService;
            _userSessionService = userSessionService;
        }

        private async void Initialize()
        {
            await LoadItems();
            //  await LoadPinnedItems();
            await CheckIfListsAreEmpty();
        }

        [RelayCommand]
        private async Task TaskAdded()
        {
            await _navigationService.NavigateWithoutRootAsync(nameof(TaskcreationPage));

        }

        private async Task LoadItems()
        {
            int userId = _userSessionService.GetUserId();

            if (userId > 0)
            {
                var tasks = await _taskService.GetUserTasksAsync(userId);
                Items.Clear();
                foreach (var task in tasks)
                {
                    Items.Add(task);
                }
            }
        }

        //private async Task LoadPinnedItems()
        //{
        //    var pinnedItems = await _database.GetItemsPinnedAysnc();
        //    PinnedItems.Clear();
        //    foreach (var item in pinnedItems)
        //    {
        //        PinnedItems.Add(item);
        //    }
        //}

        private async Task CheckIfListsAreEmpty()
        {
            IsListEmpty = !Items.Any();
            IsPinnedListEmpty = !PinnedItems.Any();
        }

        //private async Task AddItem()
        //{
        //    HapticFeedback.Perform(HapticFeedbackType.Click);
        //    await Shell.Current.GoToAsync(nameof(TodoitemPage), new ShellNavigationParameters { { "BindingContext", new Todoitem() } });
        //}

        private async Task OpenSettings()
        {
            //  await Shell.Current.GoToAsync(nameof(SettingsPage));
        }

        private async Task DeleteAllTasksAsync()
        {
            bool userConfirmed = await App.Current.MainPage.DisplayAlert("Delete All Tasks", "Are you sure you want to delete ALL tasks?", "Yes", "No");

            if (userConfirmed)
            {
                try
                {
                    int userId = _userSessionService.GetUserId();
                    var allTasks = await _taskService.GetUserTasksAsync(userId);

                    foreach (var task in allTasks)
                    {
                        await _taskService.DeleteTaskAsync(task);
                    }
                    Items.Clear();
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    var toast = Toast.Make("All Task(s) Deleted 🗑", ToastDuration.Short, 16);
                    await toast.Show(cancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "OK");
                }
            }
        }

        private async Task DeleteTaskAsync(TaskItem task)
        {
            if (task == null)
                return;

            try
            {
                await _taskService.DeleteTaskAsync(task);
                Items.Remove(task);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "OK");
            }
        }

        //private async Task SetSelectedItemStatus()
        //{
        //    var selectedItems = Items.Where(item => item.IsSelected).ToList();
        //    if (!selectedItems.Any())
        //    {
        //        await Shell.Current.DisplayAlert("No Items Selected", "Please select items to mark as complete", "OK");
        //        return;
        //    }

        //    bool allDone = selectedItems.All(item => item.Done);
        //    bool allNotDone = selectedItems.All(item => !item.Done);

        //    if (allDone)
        //    {
        //        if (await Shell.Current.DisplayAlert("Mark selected tasks as incomplete", "Do you want to mark all selected items as incomplete?", "Yes", "No"))
        //        {
        //            foreach (var item in selectedItems)
        //            {
        //                item.Done = false;
        //                item.IsSelected = false;
        //                await _database.SaveItemAsync(item);
        //            }
        //            await LoadItems();
        //        }
        //    }
        //    else if (allNotDone)
        //    {
        //        if (await Shell.Current.DisplayAlert("Mark selected tasks as complete", "Do you want to mark all selected items as complete?", "Yes", "No"))
        //        {
        //            foreach (var item in selectedItems)
        //            {
        //                item.Done = true;
        //                item.IsSelected = false;
        //                await _database.SaveItemAsync(item);
        //            }
        //            await LoadItems();
        //        }
        //    }
        //}

        //private async Task SetItemPinned()
        //{
        //    var selectedItems = Items.Where(item => item.IsSelected).ToList();
        //    if (!selectedItems.Any())
        //    {
        //        await Shell.Current.DisplayAlert("No Items Selected", "Please select items to pin", "OK");
        //        return;
        //    }

        //    bool allPinned = selectedItems.All(item => item.IsPinned);
        //    bool allUnpinned = selectedItems.All(item => !item.IsPinned);

        //    if (allPinned)
        //    {
        //        if (await Shell.Current.DisplayAlert("Unpin items", "Do you want to unpin all selected items?", "Yes", "No"))
        //        {
        //            foreach (var item in selectedItems)
        //            {
        //                item.IsPinned = false;
        //                item.IsSelected = false;
        //                await _database.SaveItemAsync(item);
        //            }
        //            await LoadItems();
        //            await LoadPinnedItems();
        //        }
        //    }
        //    else if (allUnpinned)
        //    {
        //        if (await Shell.Current.DisplayAlert("Pin items", "Do you want to pin all selected items?", "Yes", "No"))
        //        {
        //            foreach (var item in selectedItems)
        //            {
        //                item.IsPinned = true;
        //                item.IsSelected = false;
        //                await _database.SaveItemAsync(item);
        //            }
        //            await LoadItems();
        //            await LoadPinnedItems();
        //        }
        //    }
        //}

        //private async Task SetSelectedItemPriority()
        //{
        //    var selectedItems = Items.Where(item => item.IsSelected).ToList();
        //    if (!selectedItems.Any())
        //    {
        //        await Shell.Current.DisplayAlert("No Items Selected", "Please select items to set priority", "OK");
        //        return;
        //    }

        //    var priority = await Shell.Current.DisplayActionSheet("Set Priority", "Cancel", null, new[] { "Low", "Medium", "High", "Critical" });
        //    if (priority != null)
        //    {
        //        foreach (var item in selectedItems)
        //        {
        //            item.Priority = priority;
        //            item.IsSelected = false;
        //            await _database.SaveItemAsync(item);
        //        }
        //        await LoadItems();
        //    }
        //}

        private async Task RefreshItems()
        {
            await LoadItems();
            //await LoadPinnedItems();
            await CheckIfListsAreEmpty();
        }

        //private async Task SearchItems(string keyword)
        //{
        //    if (string.IsNullOrWhiteSpace(keyword))
        //    {
        //        await LoadItems();
        //    }
        //    else
        //    {
        //        var filteredItems = Items.Where(item => item.Name.ToLower().Contains(keyword.ToLower())).ToList();
        //        Items.Clear();
        //        foreach (var item in filteredItems)
        //        {
        //            Items.Add(item);
        //        }
        //    }
        //}

    }
}
