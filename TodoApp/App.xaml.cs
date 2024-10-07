using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Serilog;
using TodoApp.Services;
using TodoApp.Views;

namespace TodoApp
{
    public partial class App : Application
    {
        public static string LogFilePath { get; private set; }
        public App(INavigationService navigation,IUserService userService,IUserSessionService userSessionService)
        {
            InitializeComponent();
            LogFilePath = Path.Combine(FileSystem.AppDataDirectory, "app.log");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(LogFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });

            var logger = loggerFactory.CreateLogger<App>();
            logger.LogInformation("Application started.");
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(Dashboard), typeof(Dashboard));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(TaskcreationPage), typeof(TaskcreationPage));
            Routing.RegisterRoute(nameof(AdminDashBoard), typeof(AdminDashBoard));

            MainPage = new AppShell(new ViewModels.AppShellViewModel(userService,navigation,userSessionService));
          
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);
            window.Activated += Window_Activated;
            return window;
        }

        private async void Window_Activated(object sender, EventArgs e)
        {
#if WINDOWS


        var window = sender as Window;


        window.MinimumHeight=900;
        window.MinimumWidth=1100;


#endif
        }

        protected override void OnStart()
        {
            Log.Information("Application is starting.");
        }

        protected override void OnSleep()
        {
            Log.Information("Application is sleeping.");
        }

        protected override void OnResume()
        {
            Log.Information("Application is resuming.");
        }

    }

}
