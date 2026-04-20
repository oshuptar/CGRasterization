using System.Drawing;

namespace CGRasterization.Core.Primitives;

public class Circle
{
    public int Radius { get; set; }
    public Point  Center { get; set; }
    
    public Color Color { get; set; } = Color.Black;
    public Circle(Point center, int radius)
    {
        Radius = radius;
        Center = center;
    }
    
    public Circle(Point center, Point onCircle)
    {
        Center = center;
        Radius = (int)Math.Sqrt((Math.Pow(Math.Abs(onCircle.X - center.X), 2) + Math.Pow(Math.Abs(onCircle.Y - center.Y), 2)));
    }
}