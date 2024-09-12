
using Mopups.Services;
using TodoApp.ViewModels;

namespace TodoApp.PopupPages;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventPopup
{
        public EventPopup(EventPopupViewModel viewModel, int taskItemId)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
            _taskItemId = taskItemId;
        }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Intialize(_taskItemId);

    }
    protected bool OnBackButtonPressed()
        {
            if (MopupService.Instance.PopupStack.Count > 0)
            {
                MopupService.Instance.PopAsync();
            }
            return true;
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            if (MopupService.Instance.PopupStack.Count > 0)
            {
                MopupService.Instance.PopAsync();
            }

        }
        private EventPopupViewModel _viewModel;
        private readonly int _taskItemId;
}
