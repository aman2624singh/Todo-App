
using TodoApp.ViewModels;


namespace TodoApp.Views;

public partial class Dashboard : ContentPage
{
    bool isPanelVisible = false;
    private bool _backPressedOnce = false;
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
    protected override bool OnBackButtonPressed()
    {
        if (Navigation.NavigationStack.Count > 1 && Navigation.NavigationStack.Last() is Dashboard)
        {
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            return true; 
        }
        return base.OnBackButtonPressed();
    }
    private DashboardViewModel _viewModel;
}