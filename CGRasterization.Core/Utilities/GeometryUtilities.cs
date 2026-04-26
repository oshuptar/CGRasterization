using System.Drawing;

namespace CGRasterization.Core.Utilities;

public class GeometryUtilities
{
    public static double Distance(Point a, Point b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public static double DistanceToSegment(Point point, Point start, Point end)
    {
        double segmentX = end.X - start.X;
        double segmentY = end.Y - start.Y;
        double pointX = point.X - start.X;
        double pointY = point.Y - start.Y;
        double segmentLengthSquared = segmentX * segmentX + segmentY * segmentY;
        if (segmentLengthSquared == 0) return Distance(point, start);
        double t = (pointX * segmentX + pointY * segmentY) / segmentLengthSquared;
        t = Math.Clamp(t, 0.0, 1.0);
        double closestX = start.X + t * segmentX;
        double closestY = start.Y + t * segmentY;
        double dx = point.X - closestX;
        double dy = point.Y - closestY;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}