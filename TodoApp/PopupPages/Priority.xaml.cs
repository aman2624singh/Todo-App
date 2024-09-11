
using Mopups.Services;
using TodoApp.ViewModels;

namespace TodoApp.PopupPages;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Priority
    {
    
    private PriorityVIewModel _viewModel;
    public Priority(PriorityVIewModel viewModel)
        {
            InitializeComponent();
        BindingContext = _viewModel = viewModel;
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
