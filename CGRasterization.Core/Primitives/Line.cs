using System.Drawing;
using CGRasterization.Core.Abstractions;

namespace CGRasterization.Core.Primitives;

public class Line : IMovable, IDrawable
{
    public Point Start { get; set; }
    public Point End { get; set; }
    public int Thickness { get; set; }
    public Color Color { get; set; } = Color.Black;
    public int Dx => End.X - Start.X;
    public int Dy => End.Y - Start.Y;
    public double? Slope => Dx == 0 ? null : Dy / Dx;

    public Line(Point startPoint, Point endPoint, int  thickness = 1)
    {
        Start = startPoint;
        End = endPoint;
        Thickness = thickness;
    }
    
    public void Move(Point point)
    {
        throw new NotImplementedException();
    }
}