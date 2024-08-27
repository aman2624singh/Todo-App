using CommunityToolkit.Maui;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Mopups.Hosting;
using TodoApp.Controls;
using TodoApp.Models;
using TodoApp.Services;
using TodoApp.ViewModels;
using TodoApp.Views;

namespace TodoApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureMopups()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIconsRegular");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                }).ConfigureMauiHandlers(handlers =>
                {

                }).UseMauiCompatibility();

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IEventService, EventService>();
            builder.Services.AddSingleton<IPhotoService, PhotoService>();
            builder.Services.AddSingleton<ITaskService, TaskService>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<INavigationService, MauiNavigationService>();
            builder.Services.AddTransient<IValidator<User>, UserValidator>();
            builder.Services.AddTransient<IUserSessionService, UserSessionService>();
            //views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegistrationPage>();
            builder.Services.AddTransient<Useraccount>();
            builder.Services.AddTransient<Dashboard>();
            builder.Services.AddTransient<TaskcreationPage>();



            //viewmodels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<TaskCreationViewModel>();

            return builder.Build();
        }
    }
}
