using System.Drawing;
using CGRasterization.Core.Abstractions;

namespace CGRasterization.Core.Primitives;

public class Line : IMovable, IDrawable
{
    public Point Start { get; set; }
    public Point End { get; set; }

    public int Thickness { get; set; } = 1;
    
    public Color Color { get; set; } = Color.Black;
    public double Dx => End.X - Start.X;
    public double Dy => End.Y - Start.Y;
    
    public double? Slope => Dx == 0 ? null : Dy / Dx;

    public Line(Point startPoint, Point endPoint)
    {
        Start = startPoint;
        End = endPoint;
    }
    
    public void Move(Point point)
    {
        throw new NotImplementedException();
    }
}