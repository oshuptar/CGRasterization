using Avalonia;
using CGRasterization.App.Canvas.Tools.Abstractions;
using CGRasterization.App.Converters;
using CGRasterization.Core.Primitives;

namespace CGRasterization.App.Canvas.Tools;

public sealed class DrawLineTool : ICanvasTool
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
            new Line(
            CoordinateConverter.ToDrawingPoint(_start),
            CoordinateConverter.ToDrawingPoint(context.Position)
            ));
    }
}