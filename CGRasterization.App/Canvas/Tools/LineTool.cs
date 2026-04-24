using Avalonia;
using CGRasterization.App.Canvas.Tools.Abstractions;

namespace CGRasterization.App.Canvas.Tools;

public sealed class LineTool : ICanvasTool
{
    private Point _start;
    private bool _isDrawing;

    public void OnPointerPressed(CanvasPointerContext context)
    {
        _isDrawing = true;
        _start = context.Position;
    }

    public void OnPointerMoved(CanvasPointerContext context)
    {
        if (!_isDrawing)
            return;
        
        // context.ViewModel.SetPreviewLine(
        //     ToDrawingPoint(_start),
        //     ToDrawingPoint(context.Position));
    }

    public void OnPointerReleased(CanvasPointerContext context)
    {
        if (!_isDrawing)
            return;
        context.Pointer.Capture(null);
        _isDrawing = false;
        //context.ViewModel.ClearPreview();
        context.ViewModel.AddShape(
            ToDrawingPoint(_start),
            ToDrawingPoint(context.Position));
    }

    private static System.Drawing.Point ToDrawingPoint(Point p) => new System.Drawing.Point((int)p.X, (int)p.Y);
}