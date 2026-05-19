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
        Brush.Brush brush = new Brush.Brush(fillColor, 1);
        ScanFill(vertices, (x, y) => PutPixel(x, y, brush, buffer));
    }

    // the image is anchored top-left of the canvas and then repeated in blocks  
    public void FillImage(IReadOnlyList<Point> vertices, ImagePattern.ImagePattern pattern, PixelBuffer buffer)
    {
        ScanFill(vertices, (x, y) =>
        {
            if (x < 0 || y < 0 || x >= buffer.Width || y >= buffer.Height) return;
            int px = x % pattern.Width;
            int py = y % pattern.Height;
            int srcIdx = (py * pattern.Width + px) * 4;
            int dstIdx = y * buffer.Stride + x * buffer.BytesPerPixel;
            buffer.Pixels[dstIdx] = pattern.Pixels[srcIdx];
            buffer.Pixels[dstIdx + 1] = pattern.Pixels[srcIdx + 1];
            buffer.Pixels[dstIdx + 2] = pattern.Pixels[srcIdx + 2];
            buffer.Pixels[dstIdx + 3] = pattern.Pixels[srcIdx + 3];
        });
    }

    private void ScanFill(IReadOnlyList<Point> vertices, Action<int, int> writePixel)
    {
        if (vertices.Count < 3) return;
        int n = vertices.Count;

        int[] indices = Enumerable.Range(0, n)
            .OrderBy(idx => vertices[idx].Y)
            .ToArray();

        int yMin = vertices[indices[0]].Y;
        int yMax = vertices[indices[^1]].Y;
        if (yMin == yMax) return;

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
            aet = aet.OrderBy(e => e.X).ToList();
            for (int j = 0; j + 1 < aet.Count; j += 2)
            {
                int xStart = (int)Math.Ceiling(aet[j].X);
                int xEnd = (int)Math.Ceiling(aet[j + 1].X) - 1;
                for (int x = xStart; x <= xEnd; x++)
                    writePixel(x, y);
            }
            aet.RemoveAll(e => e.YMax == y + 1);
            for (int j = 0; j < aet.Count; j++)
            {
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
