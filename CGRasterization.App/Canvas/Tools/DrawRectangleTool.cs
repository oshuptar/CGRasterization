using Avalonia;
using CGRasterization.App.Canvas.Tools.Abstractions;
using CGRasterization.App.Converters;
using Rectangle = CGRasterization.Core.Primitives.Rectangle;

namespace CGRasterization.App.Canvas.Tools;

public sealed class DrawRectangleTool : ICanvasTool
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
        // Possible implementation of setting a preview shape
    }

    public void OnPointerReleased(CanvasPointerContext context)
    {
        if (!_isDrawing || _start is null) return;
        context.Pointer.Capture(null);
        _isDrawing = false;
        context.ViewModel.Canvas.SetPreviewShape(null);
        context.ViewModel.AddShape(
            new Rectangle(
                CoordinateConverter.ToDrawingPoint(_start.Value),
                CoordinateConverter.ToDrawingPoint(context.Position),
                context.ViewModel.Canvas.BrushColor,
                context.ViewModel.Canvas.BrushThickness));
        _start = null;
    }

    public void Cancel()
    {
        _isDrawing = false;
        _start = null;
    }
}
