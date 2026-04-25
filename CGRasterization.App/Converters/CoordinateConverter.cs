
using Avalonia;

namespace CGRasterization.App.Converters;

public class CoordinateConverter
{
    public static System.Drawing.Point ToDrawingPoint(Point p) => new System.Drawing.Point((int)p.X, (int)p.Y);
}