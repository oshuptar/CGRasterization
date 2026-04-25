using System.Drawing;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives.Abstractions;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Primitives;

public class Line : IShape
{
    public Point Start { get; set; }
    public  Point End { get; set; }
    public int Thickness { get; set; }
    public Color Color { get; set; }
    public int Dx => End.X - Start.X;
    public int Dy => End.Y - Start.Y;
    public double? Slope => Dx == 0 ? null : Dy / Dx;

    public Line(Point startPoint, Point endPoint, Color color, int thickness)
    {
        Start = startPoint;
        End = endPoint;
        Color = color;
        Thickness = thickness;
    }
    public void RasterizeWith(IShapeRasterizer rasterizer, PixelBuffer buffer) =>
        rasterizer.Rasterize(this, buffer);

    public double DistanceTo(Point point)
    {
        int thicknessRadius = Thickness / 2;
        int startX = Start.X;
        int startY = Start.Y;
        int px = point.X - startX; // vector from point to the beggining of the line
        int py = point.Y - startY;
        if (Dx * Dx + Dy * Dy == 0)
        {
            double dx = point.X - Start.X;
            double dy = point.Y - Start.Y;
            double distanceToPoint = Math.Sqrt(dx * dx + dy * dy);
            return Math.Max(distanceToPoint - thicknessRadius, 0);
        }
        double t = (double)(px * Dx + py * Dy)/(Dx * Dx + Dy * Dy);
        t = Math.Clamp(t, 0.0, 1.0);
        double qx = startX + t * Dx;
        double qy = startY + t * Dy;
        double distanceToLine = Math.Sqrt(Math.Pow(qx - point.X, 2) + Math.Pow(qy - point.Y, 2));
        return Math.Max(distanceToLine - thicknessRadius, 0);
    }
    public override string ToString() => $"Line: Start=({Start.X}, {Start.Y})\nEnd=({End.X}, {End.Y})\nDirection=({Dx}, {Dy})\nThickness={Thickness}\nColor={Color}";
    public void MoveBy(int dx, int dy)
    {
        Start = new Point(Start.X + dx, Start.Y + dy);
        End = new Point(End.X + dx, End.Y + dy);
    }
}