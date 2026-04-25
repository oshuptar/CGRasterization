using Avalonia;
using CGRasterization.App.Canvas.Tools.Abstractions;
using CGRasterization.App.Converters;
using CGRasterization.Core.Primitives;

namespace CGRasterization.App.Canvas.Tools;

public sealed class DrawCircleTool : ICanvasTool
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
        // Add potential logic here
        return;
    }

    public void OnPointerReleased(CanvasPointerContext context)
    {
        if (!_isDrawing)
            return;

        _isDrawing = false;
        context.Pointer.Capture(null);
        context.ViewModel.AddShape(
            new Circle(
                CoordinateConverter.ToDrawingPoint(_center),
                CoordinateConverter.ToDrawingPoint(context.Position),
                context.ViewModel.Canvas.Brush.Color,
                context.ViewModel.Canvas.Brush.Thickness
                ));
    }
}