
using Mopups.Services;
using TodoApp.ViewModels;

namespace TodoApp.PopupPages;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RemainderPopup
{
        public RemainderPopup(ReminderPopupViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
            
        }

       protected override void OnAppearing()
       {
        
        base.OnAppearing();

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
    private ReminderPopupViewModel _viewModel;
}
