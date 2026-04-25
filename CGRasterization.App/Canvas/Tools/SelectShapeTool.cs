using System;
using System.Linq;
using CGRasterization.App.Canvas.Tools.Abstractions;
using CGRasterization.App.Converters;
using CGRasterization.Core.Primitives.Abstractions;
using Point = System.Drawing.Point;

namespace CGRasterization.App.Canvas.Tools;

public class SelectShapeTool : ICanvasTool
{
    public void OnPointerPressed(CanvasPointerContext context)
    {
        Point clickPosition = CoordinateConverter.ToDrawingPoint(context.Position);
        IShape? selectedShape = context.ViewModel.Canvas.Shapes
            .Where(shape => shape.DistanceTo(clickPosition) <= GetSelectionTolerance(shape))
            .OrderBy(
                shape => shape.DistanceTo(clickPosition)
            )
            .FirstOrDefault();
        context.ViewModel.SelectedShape = selectedShape;
    }
    private static double GetSelectionTolerance(IShape shape)
    {
        double thicknessRadius = shape.Thickness / 2.0;
        const double baseTolerance = 7.0;
        const double decay = 4.0;
        return baseTolerance * Math.Exp(-thicknessRadius / decay);
    }

    public void OnPointerMoved(CanvasPointerContext context)
    {
    }

    public void OnPointerReleased(CanvasPointerContext context)
    {
    }
}