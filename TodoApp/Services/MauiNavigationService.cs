using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Services
{
    public class MauiNavigationService : INavigationService
    {
        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }

        public async Task NavigateToAsync(string route, IDictionary<string, object> routeParameters = null)
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                Shell.Current.Dispatcher.Dispatch(async () =>
                {
                    await Shell.Current.GoToAsync($"//{route}");
                });
            }
            else
            {
                if (routeParameters is not null)
                {
                    await Shell.Current.GoToAsync($"//{route}", routeParameters);
                }
                else
                {
                    await Shell.Current.GoToAsync($"//{route}");
                }
            }
        }

        public async Task NavigateWithoutRootAsync(string page, IDictionary<string, object> routeParameters = null)
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                Shell.Current.Dispatcher.Dispatch(async () =>
                {
                    await Shell.Current.GoToAsync(page);
                });
            }
            else
            {
                if (routeParameters is not null)
                {
                    await Shell.Current.GoToAsync(page, routeParameters);
                }
                else
                {
                    await Shell.Current.GoToAsync(page);
                }
            }
        }

        public async Task PopAsync()
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                Shell.Current.Dispatcher.Dispatch(async () =>
                {
                    await Shell.Current.Navigation.PopAsync(false);
                });
            }
            else
            {
                await Shell.Current.Navigation.PopAsync(false);
            }
        }
    }
}
