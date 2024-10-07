using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Mopups.Services;
using TodoApp.Models;
using TodoApp.PopupPages;
using TodoApp.Resources.Strings;
using TodoApp.Services;
using User = TodoApp.Models.User;

namespace TodoApp.ViewModels
{
    public partial class RegisterViewModel : ObservableObject, IQueryAttributable,IDisposable
    {
        private readonly IUserService _userService;
        private readonly IValidator<User> _userValidator;
        private readonly IAdminAuthenticationService _adminAuthenticationService;
        private readonly IUserSessionService _userSessionService;
        private readonly ILogger<RegisterViewModel> _logger;

        public RegisterViewModel(IUserService userService, ILogger<RegisterViewModel>logger ,IValidator<User> userValidator, IAdminAuthenticationService adminAuthenticationService, IUserSessionService userSessionService)
        {
            _logger=logger;
            _userService = userService;
            _userValidator = userValidator;
            GenderOptions = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
            _adminAuthenticationService = adminAuthenticationService;
            _userSessionService = userSessionService;
        }

        [ObservableProperty]
        private int currentUserId;

        [ObservableProperty]
        private Gender _selectedGender;

        public List<Gender> GenderOptions { get; }

        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string phoneNumber;

        [ObservableProperty]
        private DateTime dateOfBirth = DateTime.Today;

        [ObservableProperty]
        private string registerMessage;

        [RelayCommand]
        private async Task RegisterClicked()
        {
            try
            {
                var user = new User
                {
                    UserName = UserName,
                    Email = Email,
                    Password = Password,
                    PhoneNumber = PhoneNumber,
                    Gender = SelectedGender.ToString()
                };

                var validationResult = _userValidator.Validate(user);

                if (!validationResult.IsValid)
                {
                    var errorMessage = string.Join("<br/>", validationResult.Errors.Select(e => e.ErrorMessage));
                    await MopupService.Instance.PushAsync(new ErrorPopup(errorMessage));
                    return;
                }

                if (_userSessionService.IsAdmin())
                {
                    var existingUser = await _userService.GetUserByIdAsync(CurrentUserId);

                    if (existingUser is not null)
                    {
                        existingUser.UserName = user.UserName;
                        existingUser.Email = user.Email;
                        existingUser.Password = user.Password;
                        existingUser.PhoneNumber = user.PhoneNumber;
                        existingUser.Gender = user.Gender;

                        await _userService.UpdateUserAsync(existingUser);
                        await Toast.Make(AppstringResources.UserUpdated, ToastDuration.Short).Show();
                    }
                    else
                    {
                        await MopupService.Instance.PushAsync(new ErrorPopup(AppstringResources.UsernotExist));
                    }

                    await Shell.Current.GoToAsync("//AdminDashBoard");
                }
                else
                {
                    var existingUser = await _userService.GetUserAsync(user.UserName, user.Password);
                    if (existingUser is not null)
                    {
                        await MopupService.Instance.PushAsync(new ErrorPopup(AppstringResources.Userexist));
                        return;
                    }

                    await _userService.AddUserAsync(user);
                    await Toast.Make(AppstringResources.RegistrationSuccsess, ToastDuration.Short).Show();
                    await Shell.Current.GoToAsync("///LoginPage");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,string.Empty);
                await MopupService.Instance.PushAsync(new ErrorPopup(AppstringResources.ErrorOccured));
            }
        }

        [RelayCommand]
        private async Task SigninClicked()
        {
            try
            {
                await Shell.Current.GoToAsync("///LoginPage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,string.Empty);
            }
        }


        public void Dispose()
        {
            UserName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            PhoneNumber=string.Empty;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if(query.ContainsKey("UserId"))
                CurrentUserId = int.Parse(query["UserId"].ToString());

            if (query.ContainsKey("UserName"))
                UserName = query["UserName"] as string;

            if (query.ContainsKey("Password"))

                Password = query["Password"] as string;

            if (query.ContainsKey("Email"))
                Email = query["Email"] as string;

            if (query.ContainsKey("PhoneNumber"))
                PhoneNumber = query["PhoneNumber"] as string;

            if (query.ContainsKey("Gender") && Enum.TryParse(typeof(Gender), query["Gender"] as string, out var gender))
            {
                SelectedGender = (Gender)gender;
            }
        }
    }
}
