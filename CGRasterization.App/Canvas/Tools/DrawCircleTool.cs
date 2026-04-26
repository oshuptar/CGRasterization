using Avalonia;
using CGRasterization.App.Canvas.Tools.Abstractions;
using CGRasterization.App.Converters;
using CGRasterization.Core.Primitives;

namespace CGRasterization.App.Canvas.Tools;

public sealed class DrawCircleTool : ICanvasTool
{
    private Point? _center;
    private bool _isDrawing;

    public void OnPointerPressed(CanvasPointerContext context)
    {
        _isDrawing = true;
        _center = context.Position;
        context.Pointer.Capture(context.Canvas);
    }
    public void OnPointerMoved(CanvasPointerContext context)
    {
        // Add potential logic here
        return;
    }

    public void OnPointerReleased(CanvasPointerContext context)
    {
        if (!_isDrawing || _center is null) return;
        _isDrawing = false;
        context.Pointer.Capture(null);
        context.ViewModel.AddShape(
            new Circle(
                CoordinateConverter.ToDrawingPoint(_center.Value),
                CoordinateConverter.ToDrawingPoint(context.Position),
                context.ViewModel.Canvas.Brush.Color,
                context.ViewModel.Canvas.Brush.Thickness
                ));
    }
    public void Cancel()
    {
        _isDrawing = false;
        _center = null;
    }
}