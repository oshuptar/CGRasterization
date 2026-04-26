using System;
using System.Drawing;
using CGRasterization.App.Canvas.Tools.Abstractions;
using CGRasterization.App.Converters;
using CGRasterization.Core.Primitives.Abstractions;
using CGRasterization.Core.ShapeHandles.Asbtractions;

namespace CGRasterization.App.Canvas.Tools;

public class EditShapeTool : ICanvasTool
{
    private IShape? _editingShape;
    private IShapeHandle? _activeHandle;
    private Point _lastPosition;
    private bool _isEditing;

    public void OnPointerPressed(CanvasPointerContext context)
    {
        if (context.ViewModel.SelectedShape is null) return;
        Point clickPosition = CoordinateConverter.ToDrawingPoint(context.Position);
        IShapeHandle? handle = context.ViewModel.SelectedShape.GetHandle(clickPosition, GetSelectionTolerance(context.ViewModel.SelectedShape));
        if (handle is null) return;
        _editingShape = context.ViewModel.SelectedShape;
        _activeHandle = handle;
        _lastPosition = clickPosition;
        _isEditing = true;
        context.Pointer.Capture(context.Canvas);
    }

    public void OnPointerMoved(CanvasPointerContext context)
    {
        if (!_isEditing || _editingShape is null || _activeHandle is null) return;
        Point currentPosition = CoordinateConverter.ToDrawingPoint(context.Position);
        int dx = currentPosition.X - _lastPosition.X;
        int dy = currentPosition.Y - _lastPosition.Y;
        if (dx == 0 && dy == 0) return;
        _editingShape.MoveHandle(_activeHandle, dx, dy);
        _lastPosition = currentPosition;
        context.ViewModel.Canvas.RedrawShapes();
        context.ViewModel.RefreshSelected();
    }

    public void OnPointerReleased(CanvasPointerContext context)
    {
        if (!_isEditing) return;
        _isEditing = false;
        _editingShape = null;
        _activeHandle = null;
        context.Pointer.Capture(null);
    }
    
    private static double GetSelectionTolerance(IShape shape)
    {
        double thicknessRadius = shape.Thickness / 2.0;
        const double baseTolerance = 7.0;
        const double decay = 4.0;
        return baseTolerance * Math.Exp(-thicknessRadius / decay);
    }

    public void Cancel()
    {
        _editingShape = null;
        _activeHandle = null;
        _isEditing = false;
    }
}