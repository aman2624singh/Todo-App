
using TodoApp.ViewModels;


namespace TodoApp.Views;

public partial class Dashboard : ContentPage
{
    bool isPanelVisible = false;
    public Dashboard(DashboardViewModel viewModel)
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

    private DashboardViewModel _viewModel;
}