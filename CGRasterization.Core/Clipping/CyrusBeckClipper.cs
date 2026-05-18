using System.Drawing;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Primitives.Abstractions;
using Rectangle = CGRasterization.Core.Primitives.Rectangle;

namespace CGRasterization.Core.Clipping;

public static class CyrusBeckClipper
{
    public static bool IsConvex(IReadOnlyList<Point> polygon)
    {
        if (polygon.Count < 3) return false;
        int n = polygon.Count;
        int sign = 0;
        for (int i = 0; i < n; i++)
        {
            Point a = polygon[i];
            Point b = polygon[(i + 1) % n];
            Point c = polygon[(i + 2) % n];
            int cross = (b.X - a.X) * (c.Y - b.Y) - (b.Y - a.Y) * (c.X - b.X);
            if (cross == 0) continue;
            int currentSign = cross > 0 ? 1 : -1;
            if (sign == 0) sign = currentSign;
            else if (sign != currentSign) return false;
        }
        return sign != 0;   
    }
    // Returns the clipped segment [p1, p2] against a convex clip polygon, or null if fully outside.
    public static (Point p1, Point p2)? Clip(Point p1, Point p2, IReadOnlyList<Point> clipPolygon)
    {
        clipPolygon = NormalizeCounterClockwiseScreenOrder(clipPolygon);
        if (!IsConvex(clipPolygon))
            return null;
        double tE = 0.0;
        double tL = 1.0;
        int dx = p2.X - p1.X;
        int dy = p2.Y - p1.Y;
        int n = clipPolygon.Count;
        for (int i = 0; i < n; i++)
        {
            Point edgeStart = clipPolygon[i];
            Point edgeEnd   = clipPolygon[(i + 1) % n];
            
            // left edge: in screen coordinates points outside assuming counter-clockwise edge direction
            int nx = edgeEnd.Y - edgeStart.Y;
            int ny = -(edgeEnd.X - edgeStart.X);
            double denom = (nx * dx + ny * dy);
            double nom = nx * (p1.X - edgeStart.X) + ny * (p1.Y - edgeStart.Y);
            if (denom == 0)
            {
                // With outward normal: Segment is parallel to this edge; outside if nom > 0.
                if (nom > 0) return null;
                continue;
            }
            double t = -nom / denom;
            if (denom < 0)
                tE = Math.Max(tE, t); // entering
            else
                tL = Math.Min(tL, t); // leaving
            if (tE > tL) return null;
        }
        return (
            new Point((int)(p1.X + tE * dx), (int)(p1.Y + tE * dy)),
            new Point((int)(p1.X + tL * dx), (int)(p1.Y + tL * dy))
        );
    }

    public static IEnumerable<(Point p1, Point p2)> ClipPolygonEdges(Polygon source, IReadOnlyList<Point> clipWindow)
    {
        int n = source.Vertices.Count;
        for (int i = 0; i < n; i++)
        {
            Point a = source.Vertices[i];
            Point b = source.Vertices[(i + 1) % n];
            // Skip closing edge for open polygons.
            if (!source.IsClosed && i == n - 1) break;
            var clipped = Clip(a, b, clipWindow);
            if (clipped.HasValue)
                yield return clipped.Value;
        }
    }

    public static IReadOnlyList<Point>? GetClipWindowPoints(IShape shape) => shape switch
    {
        Polygon { IsClosed: true, Vertices.Count: >= 3 } p => p.Vertices,
        Rectangle r => new[] { r.TopLeft, r.TopRight, r.BottomRight, r.BottomLeft },
        _ => null
    };
    
    private static double SignedArea(IReadOnlyList<Point> polygon)
    {
        double area = 0;
        for (int i = 0; i < polygon.Count; i++)
        {
            Point a = polygon[i];
            Point b = polygon[(i + 1) % polygon.Count];
            area += a.X * b.Y - b.X * a.Y;
        }
        return area / 2.0;
    }

    private static IReadOnlyList<Point> NormalizeCounterClockwiseScreenOrder(IReadOnlyList<Point> polygon)
    {
        // In screen coordinates desired order has positive signed area:
        // TopLeft -> TopRight -> BottomRight -> BottomLeft.
        if (SignedArea(polygon) > 0) return polygon;
        return polygon.Reverse().ToArray();
    }
}
