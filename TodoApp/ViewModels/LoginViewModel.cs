using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoApp.Constant;
using TodoApp.Models;
using TodoApp.Resources.Strings;
using TodoApp.Services;
using TodoApp.Views;

namespace TodoApp.ViewModels
{
    public partial class LoginViewModel:ObservableObject
    {
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSessionService;
 

        public LoginViewModel(IUserService userService, INavigationService navigationService, IUserSessionService userSessionService)
        {
            _userService = userService;
            _navigationService = navigationService;
            _userSessionService = userSessionService;

        }

        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    if (!string.IsNullOrWhiteSpace(value) &&
                        LoginErrorMessage == AppstringResources.Error_Username)
                    {
                        LoginErrorMessage = string.Empty;
                        IsLoginErrorVisible = false;
                    }
                    else if (string.IsNullOrWhiteSpace(value))
                    {
                        LoginErrorMessage = AppstringResources.Error_Username;
                        IsLoginErrorVisible = true;
                    }
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    if (!string.IsNullOrWhiteSpace(value) &&
                        LoginErrorMessage == AppstringResources.Error_password)
                    {
                        LoginErrorMessage = string.Empty;
                        IsLoginErrorVisible = false;
                    }
                    else if (string.IsNullOrWhiteSpace(value))
                    {
                        LoginErrorMessage = AppstringResources.Error_password;
                        IsLoginErrorVisible = true;
                    }
                }
            }
        }

        [ObservableProperty]
        private bool isLoginErrorVisible;

        [ObservableProperty]
        private string passwordVisibilityIcon;

        [ObservableProperty]
        private bool isPassword;

        [ObservableProperty]
        private string loginErrorMessage = string.Empty;


        [RelayCommand]
        private async Task LoginClicked()
        {
            try
            {
                IsLoginErrorVisible = false;
           

                if (!HasValidInput())
                {
                    return;
                }

                var user = await _userService.AuthenticateUserAsync(Username, Password);

                if (user != null)
                {
                    _userSessionService.SetUserId(user.Id);
                    _userSessionService.SetUserName(user.UserName);
                    await Shell.Current.GoToAsync("//DashboardPage", true);
                }
                else
                {
                    LoginErrorMessage = AppstringResources.Error_login;
                    IsLoginErrorVisible = true;
                }
            }
            catch (Exception ex)
            {
                LoginErrorMessage = AppstringResources.Error_login;
                IsLoginErrorVisible = true;
            }
            finally
            {
                
            }
        }



        [RelayCommand]
        private async Task CreateAccount()
        {
          await _navigationService.NavigateWithoutRootAsync(nameof(RegistrationPage));
        }

        private bool HasValidInput()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                LoginErrorMessage = AppstringResources.Error_Username;
                IsLoginErrorVisible = true;
                return false;
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                LoginErrorMessage = AppstringResources.Error_password;
                IsLoginErrorVisible = true;
                return false;
            }

            return true;
        }

        public void InitializeLoginDetails()
        {
            IsPassword = true;
            PasswordVisibilityIcon = Icons.EyeHideIcon;
        }

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPassword = !IsPassword;
            PasswordVisibilityIcon = IsPassword ? Icons.EyeHideIcon : Icons.EyeIcon;
        }

        private string _username= "aman123";
        private string _password="test@123" ;
    }
}
