using Avalonia;
using CGRasterization.App.Canvas.Tools.Abstractions;

namespace CGRasterization.App.Canvas.Tools;

public sealed class CircleTool : ICanvasTool
{
    private Point _center;
    private bool _isDrawing;

    public void OnPointerPressed(CanvasPointerContext context)
    {
        _isDrawing = true;
        _center = context.Position;
    }
    public void OnPointerMoved(CanvasPointerContext context)
    {
        if (!_isDrawing)
            return;
    }

    public void OnPointerReleased(CanvasPointerContext context)
    {
        if (!_isDrawing)
            return;

        _isDrawing = false;
        context.Pointer.Capture(null);
        context.ViewModel.AddShape(
            ToDrawingPoint(_center),
            ToDrawingPoint(context.Position));
    }

    private static System.Drawing.Point ToDrawingPoint(Point p)
    {
        return new System.Drawing.Point((int)p.X, (int)p.Y);
    }
}