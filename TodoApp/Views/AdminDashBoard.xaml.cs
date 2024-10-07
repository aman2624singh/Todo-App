
using TodoApp.Models;
using TodoApp.ViewModels;

namespace TodoApp.Views;

public partial class AdminDashBoard : ContentPage
{
    public AdminDashBoard(AdminDashBoardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Initilaize();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is MenuFlyoutItem menuItem && menuItem.CommandParameter is User user)
        {
            await _viewModel.EditUser(user);
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is MenuFlyoutItem menuItem && menuItem.CommandParameter is User user)
        {
            await _viewModel.DeleteUser(user);
        }
    }

    protected override bool OnBackButtonPressed()
    {
        if (Navigation.NavigationStack.Count > 1 && Navigation.NavigationStack.Last() is AdminDashBoard)
        {
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            return true;
        }
        return base.OnBackButtonPressed();
    }

    private AdminDashBoardViewModel _viewModel;
}