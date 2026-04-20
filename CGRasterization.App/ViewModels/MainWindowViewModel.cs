using CGRasterization.App.ViewModels.Abstractions;

namespace CGRasterization.App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public CanvasControlViewModel CanvasControlViewModel { get; set; } = new();
}