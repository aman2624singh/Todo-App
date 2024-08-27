using TodoApp.Views;

namespace TodoApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(Dashboard), typeof(Dashboard));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(Useraccount), typeof(Useraccount));
            Routing.RegisterRoute(nameof(TaskcreationPage), typeof(TaskcreationPage));
            MainPage = new AppShell();
        }
    }
}
