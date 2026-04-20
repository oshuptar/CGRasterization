using System.Drawing;
using CGRasterization.App.ViewModels.Abstractions;
using CGRasterization.Core.Primitives;

namespace CGRasterization.App.ViewModels;

public class CanvasControlViewModel : ViewModelBase
{
    public Canvas.Canvas Canvas { get; set; } = new(1200, 800);

    public void AddLine(Point startPoint, Point endPoint)
    {
        Line newLine = new Line(startPoint, endPoint);
        Canvas.Lines.Add(newLine);
    }
}