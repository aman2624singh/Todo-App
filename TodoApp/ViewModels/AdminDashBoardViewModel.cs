using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models;
using TodoApp.Resources.Strings;
using TodoApp.Services;
using TodoApp.Views;

namespace TodoApp.ViewModels
{
    public partial class AdminDashBoardViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSessionService;


        [ObservableProperty]
        private string username;

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


        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

        public AdminDashBoardViewModel(IUserService userService, INavigationService navigationService,IUserSessionService userSessionService)
        {
            _userService = userService;
            _navigationService = navigationService;
            _userSessionService = userSessionService;
            Username = AppstringResources.admin;

        }

        public void Initilaize()
        {
            LoadUsers();
        }

        private async void LoadUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        [RelayCommand]
        public async Task DeleteUser(User user)
        {
            if (user is null) return;

            bool userConfirmed = await App.Current.MainPage.DisplayAlert(
                    AppstringResources.Deletetask,
                    $"Are you sure you want to delete the User: {user.UserName}?",
                    AppstringResources.Yes,
                    AppstringResources.No
                );
            if (userConfirmed)
            {
                await _userService.DeleteUserAsync(user.Id);
                Users.Remove(user);
            }
        }

        [RelayCommand]
        public async Task EditUser(User user)
        {
            if (user is null) return;

            await Shell.Current.GoToAsync($"RegistrationPage", new Dictionary<string, object>
    {
                {"UserId",user.Id.ToString() },
        { "UserName", user.UserName },
        { "Password", user.Password },
        { "Email", user.Email },
        { "PhoneNumber", user.PhoneNumber },
        { "Gender", user.Gender.ToString() }
    });
        }

        private void SearchTasks()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Initilaize();
            }
            else
            {
                var filteredItems = Users.Where(user =>user.UserName .Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                Users.Clear();
                filteredItems.ToList().ForEach(user => Users.Add(user));
            }
        }

        [RelayCommand]
        private async Task TaskTapped(User user)
        {
            if (user == null) return;
            _userSessionService.SetUserId(user.Id);           
            await Shell.Current.GoToAsync(nameof(Dashboard));
        }
    }
}
