using System.Drawing;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives.Abstractions;
using CGRasterization.Core.Rasterizers.Abstractions;
using CGRasterization.Core.ShapeHandles;
using CGRasterization.Core.ShapeHandles.Asbtractions;

namespace CGRasterization.Core.Primitives;

public class Circle : IShape
{
    public int Radius { get; set; }
    public Point  Center { get; set; }
    public int Thickness { get; set; }
    public Color Color { get; set; }
    public Circle(Point center, int radius)
    {
        Radius = radius;
        Center = center;
    }
    
    public Circle(Point center, Point onCircle, Color color, int thickness)
    {
        Center = center;
        Radius = (int)Math.Sqrt((Math.Pow(Math.Abs(onCircle.X - center.X), 2) + Math.Pow(Math.Abs(onCircle.Y - center.Y), 2)));
        Color = color;
        Thickness = thickness;
    }
    public void RasterizeWith(IShapeRasterizer rasterizer, PixelBuffer buffer) => rasterizer.Rasterize(this, buffer);
    public double DistanceTo(Point point)
    {
        int thicknessRadius = Thickness / 2;
        double dx = point.X - Center.X;
        double dy = point.Y - Center.Y;
        double distanceFromCenter = Math.Sqrt(dx * dx + dy * dy);
        double distanceToCircleLine = Math.Abs(distanceFromCenter - Radius);
        return Math.Max(0, distanceToCircleLine - thicknessRadius);
    }
    public override string ToString() => $"Circle: Center=({Center.X}, {Center.Y})\nRadius={Radius}";
    
    public void MoveBy(int dx, int dy) => Center = new Point(Center.X + dx, Center.Y + dy);
    
    public IShapeHandle? GetHandle(Point point, double tolerance)
    {
        double dx = point.X - Center.X;
        double dy = point.Y - Center.Y;
        double distanceFromCenter = Math.Sqrt(dx * dx + dy * dy);
        double distanceToCircle = Math.Abs(distanceFromCenter - Radius);
        if (distanceToCircle > tolerance) return null;
        Point handlePosition = point;
        return new ShapeHandle(
            getPosition: () => handlePosition,
            setPosition: newPosition =>
            {
                handlePosition = newPosition;
                double newDx = newPosition.X - Center.X;
                double newDy = newPosition.Y - Center.Y;
                Radius = (int)Math.Round(Math.Sqrt(newDx * newDx + newDy * newDy));
            });
    }
    public void MoveHandle(IShapeHandle handle, int dx, int dy) => handle.MoveBy(dx, dy);

}