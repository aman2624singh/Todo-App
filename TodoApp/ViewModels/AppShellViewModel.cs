using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Services;

namespace TodoApp.ViewModels
{
    public partial class AppShellViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly Shell _shell;
        public AppShellViewModel(IUserService userService, INavigationService navigationService)
        {
            _userService = userService;
            _navigationService = navigationService;

        }


        [RelayCommand]
        private async Task Logout()
        {
     
            _userService.Logout();
            Shell.Current.FlyoutIsPresented = false;
            
            await _navigationService.NavigateToAsync("//LoginPage");
        }

        //[RelayCommand]
        //private async Task SendLogs()
        //{
        //    // Logic to send logs
        //    await _logService.SendLogsAsync();
        //    await Application.Current.MainPage.DisplayAlert("Logs Sent", "Your logs have been sent successfully.", "OK");
        //}
    }
}
