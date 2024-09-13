using The49.Maui.BottomSheet;
using TodoApp.ViewModels;

namespace TodoApp.Views;

public partial class EventBottomSheet : BottomSheet
{
	public EventBottomSheet(EventBottomSheetViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private EventBottomSheetViewModel _viewModel;
}