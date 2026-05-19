using System.Drawing;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Rasterizers;

public class PolygonFillRasterizer : BaseRasterizer
{
    private struct EdgeEntry
    {
        public int YMax;
        public float X;
        public float InvSlope;
    }

    public void Fill(IReadOnlyList<Point> vertices, Color fillColor, PixelBuffer buffer)
    {
        if (vertices.Count < 3) return;
        int n = vertices.Count;
        
        int[] indices = Enumerable.Range(0, n)
            .OrderBy(idx => vertices[idx].Y)
            .ToArray();

        int yMin = vertices[indices[0]].Y;
        int yMax = vertices[indices[^1]].Y;
        if (yMin == yMax) return;

        Brush.Brush brush = new Brush.Brush(fillColor, 1);
        var aet = new List<EdgeEntry>();
        int k = 0;
        for (int y = yMin; y < yMax; y++)
        {
            while (k < n && vertices[indices[k]].Y == y)
            {
                int vi = indices[k];
                Point cur = vertices[vi];
                Point prev = vertices[(vi - 1 + n) % n]; // in C# -1 % n = -1
                Point next = vertices[(vi + 1) % n];
                if (prev.Y > y) aet.Add(MakeEdge(cur, prev));
                if (next.Y > y) aet.Add(MakeEdge(cur, next));
                k++;
            }
            aet = aet.OrderBy(edgeEntry => edgeEntry.X).ToList();
            for (int j = 0; j + 1 < aet.Count; j += 2)
            {
                int xStart = (int)Math.Ceiling(aet[j].X);
                int xEnd = (int)Math.Ceiling(aet[j + 1].X) - 1;
                for (int x = xStart; x <= xEnd; x++)
                    PutPixel(x, y, brush, buffer);
            }

            // Remove edges whose upper vertex is reached at the next scan line
            aet.RemoveAll(e => e.YMax == y + 1);
            for (int j = 0; j < aet.Count; j++)
            {
                // Structs are immutable
                EdgeEntry e = aet[j];
                e.X += e.InvSlope;
                aet[j] = e;
            }
        }
    }

    private static EdgeEntry MakeEdge(Point start, Point end)
    {
        return new EdgeEntry
        {
            YMax = end.Y,
            X = start.X,
            InvSlope = (float)(end.X - start.X) / (end.Y - start.Y)
        };
    }
}
