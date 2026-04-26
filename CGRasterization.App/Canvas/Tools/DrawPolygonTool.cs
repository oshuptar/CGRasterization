using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CGRasterization.App.Canvas.Tools.Abstractions;
using CGRasterization.App.Converters;
using CGRasterization.Core.Brush;
using CGRasterization.Core.Primitives;

namespace CGRasterization.App.Canvas.Tools;

public class DrawPolygonTool : ICanvasTool
{
    private readonly List<Point> _points = new();
    private bool _isDragging;

    public void OnPointerPressed(CanvasPointerContext context)
    {
        Point position = CoordinateConverter.ToDrawingPoint(context.Position);
        if (_points.Count == 0)
            _points.Add(position);
        _isDragging = true;
        context.Pointer.Capture(context.Canvas);
    }

    public void OnPointerMoved(CanvasPointerContext context)
    {
        if (!_isDragging) return;
        Point currentPosition = CoordinateConverter.ToDrawingPoint(context.Position);
        var previewPoints = _points.Append(currentPosition).ToList();
        SetPreview(context, previewPoints, isClosed: false);
    }

    public void OnPointerReleased(CanvasPointerContext context)
    {
        if (!_isDragging) return;
        _isDragging = false;
        context.Pointer.Capture(null);
        Point releasedPoint = CoordinateConverter.ToDrawingPoint(context.Position);
        if (_points.Count >= 3 && IsNear(releasedPoint, _points[0], GetSelectionTolerance(context.ViewModel.Canvas.Brush)))
        {
            ClosePolygon(context);
            return;
        }
        if (releasedPoint == _points[^1]) return;
        _points.Add(releasedPoint);
        UpdatePreview(context);
    }
    private void UpdatePreview(CanvasPointerContext context)
    {
        if (_points.Count < 2)
        {
            context.ViewModel.Canvas.SetPreviewShape(null);
            return;
        }
        SetPreview(context, _points.ToList(), isClosed: false);
    }

    private void SetPreview(CanvasPointerContext context, List<Point> points, bool isClosed)
    {
        context.ViewModel.Canvas.SetPreviewShape(new Polygon(
                points,
                isClosed,
                context.ViewModel.Canvas.Brush.Color,
                context.ViewModel.Canvas.Brush.Thickness));
    }

    private void ClosePolygon(CanvasPointerContext context)
    {
        if (_points.Count < 3) return;
        var finalVertices = _points.ToList();
        var polygon = new Polygon(finalVertices, true, context.ViewModel.Canvas.Brush.Color, context.ViewModel.Canvas.Brush.Thickness);
        _points.Clear();
        _isDragging = false;
        context.ViewModel.Canvas.SetPreviewShape(null);
        context.ViewModel.AddShape(polygon);
    }

    private static bool IsNear(Point a, Point b, double tolerance)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy) <= tolerance;
    }

    private static double GetSelectionTolerance(Brush brush, double baseTolerance = 20.0)
    {
        double thicknessRadius = brush.Thickness / 2.0;
        const double decay = 4.0;
        return baseTolerance * Math.Exp(-thicknessRadius / decay);
    }
    public void Cancel()
    {
        _points.Clear();
        _isDragging = false;
    }
}