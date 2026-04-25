using CGRasterization.App.ViewModels.Abstractions;
using CommunityToolkit.Mvvm.Input;

namespace CGRasterization.App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public CanvasControlViewModel CanvasControlViewModel { get; set; } = new();
    public RelayCommand ClearBitmapCommand { get; }

    public MainWindowViewModel()
    {
        ClearBitmapCommand = new RelayCommand(() => CanvasControlViewModel.Canvas.ClearCanvas());
    }
}