using CGRasterization.App.ViewModels.Abstractions;

namespace CGRasterization.App.ViewModels;

public class CanvasControlViewModel : ViewModelBase
{
    public Canvas.Canvas Canvas { get; set; } = new(1200, 800);
}