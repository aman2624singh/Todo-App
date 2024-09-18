using TodoApp.Models;
using TodoApp.ViewModels;
using TodoApp.Views;

namespace TodoApp
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel vm)
        {
            InitializeComponent();

            // Set the BindingContext
            BindingContext = vm;
        }

    }
}
