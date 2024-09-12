
using TodoApp.ViewModels;

namespace TodoApp.Views;

public partial class TaskcreationPage : ContentPage
{
    public TaskcreationPage(TaskCreationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Initialize();

    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    private TaskCreationViewModel _viewModel;
}