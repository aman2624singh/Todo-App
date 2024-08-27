
using TodoApp.ViewModels;


namespace TodoApp.Views;

public partial class Dashboard : ContentPage
{
    public Dashboard(DashboardViewModel viewModel)
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
    }


    private DashboardViewModel _viewModel;
}