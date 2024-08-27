
using TodoApp.ViewModels;

namespace TodoApp.Views;

public partial class RegistrationPage : ContentPage
{
    public RegistrationPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Dispose();
    }

    private RegisterViewModel _viewModel;
}