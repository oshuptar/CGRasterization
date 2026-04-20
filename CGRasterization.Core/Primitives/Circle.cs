using System.Drawing;

namespace CGRasterization.Core.Primitives;

public class Circle
{
    public int Radius { get; set; }
    public Point  Center { get; set; }
    
    public Circle(int radius, Point center)
    {
        Radius = radius;
        Center = center;
    }
}