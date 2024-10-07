using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
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
        private readonly IUserSessionService _userSessionService;
        private readonly Shell _shell;
        public AppShellViewModel(IUserService userService, INavigationService navigationService, IUserSessionService userSessionService)
        {
            _userService = userService;
            _navigationService = navigationService;
            _userSessionService = userSessionService;
        }


        [RelayCommand]
        private async Task Logout()
        {
            _userService.Logout();
            _userSessionService.SetIsAdmin(false);
            Shell.Current.FlyoutIsPresented = false;
             await _navigationService.NavigateToAsync("//LoginPage");
            
        }

    }
}
