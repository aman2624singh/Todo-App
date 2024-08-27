
using TodoApp.ViewModels;

namespace TodoApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.InitializeLoginDetails();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    private LoginViewModel _viewModel;
}