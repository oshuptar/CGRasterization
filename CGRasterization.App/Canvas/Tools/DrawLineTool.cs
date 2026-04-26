using Avalonia;
using CGRasterization.App.Canvas.Tools.Abstractions;
using CGRasterization.App.Converters;
using CGRasterization.Core.Primitives;

namespace CGRasterization.App.Canvas.Tools;

public sealed class DrawLineTool : ICanvasTool
{
    private Point? _start;
    private bool _isDrawing;

    public void OnPointerPressed(CanvasPointerContext context)
    {
        _isDrawing = true;
        _start = context.Position;
        context.Pointer.Capture(context.Canvas);
    }
    public void OnPointerMoved(CanvasPointerContext context)
    {
        if (!_isDrawing) return;
    }
    public void OnPointerReleased(CanvasPointerContext context)
    {
        if (!_isDrawing || _start == null) return;
        context.Pointer.Capture(null);
        _isDrawing = false;
        context.ViewModel.AddShape(
            new Line(
            CoordinateConverter.ToDrawingPoint(_start.Value),
            CoordinateConverter.ToDrawingPoint(context.Position),
            context.ViewModel.Canvas.Brush.Color,
            context.ViewModel.Canvas.Brush.Thickness
            ));
    }
    public void Cancel()
    {
       _isDrawing = false;
       _start = null;
    }
}