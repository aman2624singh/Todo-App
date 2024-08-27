
using Mopups.Services;

namespace TodoApp.PopupPages;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorPopup
    {
        public ErrorPopup(string errorMessage)
        {
            InitializeComponent();
            ErrorMessage.Text = errorMessage;
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
    }
