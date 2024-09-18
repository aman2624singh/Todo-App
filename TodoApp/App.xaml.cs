using Plugin.LocalNotification;
using TodoApp.Services;
using TodoApp.Views;

namespace TodoApp
{
    public partial class App : Application
    {

        public App(INavigationService navigation,IUserService userService)
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(Dashboard), typeof(Dashboard));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(Useraccount), typeof(Useraccount));
            Routing.RegisterRoute(nameof(TaskcreationPage), typeof(TaskcreationPage));

            MainPage = new AppShell(new ViewModels.AppShellViewModel(userService,navigation));
          
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



    }

}
