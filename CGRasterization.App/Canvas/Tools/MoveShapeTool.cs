using CGRasterization.App.Canvas.Enums;
using CGRasterization.App.Canvas.Tools.Abstractions;
using CGRasterization.App.Converters;
using CGRasterization.Core.Primitives.Abstractions;
using Point = System.Drawing.Point;

namespace CGRasterization.App.Canvas.Tools;

public class MoveShapeTool : ICanvasTool
{
    private IShape? _movingShape;
    private Point _lastPosition;
    private bool _isMoving;

    public void OnPointerPressed(CanvasPointerContext context)
    {
        if (context.ViewModel.SelectedShape is null) return;
        _movingShape = context.ViewModel.SelectedShape;
        _lastPosition = CoordinateConverter.ToDrawingPoint(context.Position);
        _isMoving = true;
        context.Pointer.Capture(context.Canvas);
    }

    public void OnPointerMoved(CanvasPointerContext context)
    {
        if (!_isMoving || _movingShape is null) return;
        Point currentPosition = CoordinateConverter.ToDrawingPoint(context.Position);
        int dx = currentPosition.X - _lastPosition.X;
        int dy = currentPosition.Y - _lastPosition.Y;
        if (dx == 0 && dy == 0) return;
        _movingShape.MoveBy(dx, dy);
        _lastPosition = currentPosition;
        context.ViewModel.Canvas.RedrawShapes();
        context.ViewModel.RefreshSelected();
    }
    public void OnPointerReleased(CanvasPointerContext context)
    {
        if(!_isMoving || _movingShape is null) return;
        _isMoving = false;
        _movingShape = null;
        context.Pointer.Capture(null);
        context.ViewModel.SelectedToolType = CanvasToolType.SelectShape;
    }

    public void Cancel()
    {
        _movingShape = null;
        _isMoving = false;
    }
}