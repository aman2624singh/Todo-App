using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using Mopups.Services;
using TodoApp.Models;
using TodoApp.PopupPages;
using TodoApp.Services;

namespace TodoApp.ViewModels
{
    public partial class RegisterViewModel : ObservableObject,IDisposable
    {
        private readonly IUserService _userService;
        private readonly IValidator<User> _userValidator;

        public RegisterViewModel(IUserService userService, IValidator<User> userValidator)
        {
            _userService = userService;
            _userValidator = userValidator;
            GenderOptions = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();

        }


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

            var existingUser = await _userService.GetUserAsync(user.UserName, user.Password);
            if (existingUser != null)
            {
                await MopupService.Instance.PushAsync(new ErrorPopup("User already exists."));
                return;
            }

            await _userService.AddUserAsync(user);
            await Toast.Make("Registration successful! You can now log in.", ToastDuration.Short).Show(); 
            await Shell.Current.GoToAsync("///LoginPage");
        }

        [RelayCommand]
        private async Task SigninClicked()
        {
            await Shell.Current.GoToAsync("///LoginPage");
        }

        public void Dispose()
        {
            UserName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            PhoneNumber=string.Empty;
        }
    }
}
