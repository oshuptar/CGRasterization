using System.Drawing;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives.Abstractions;
using CGRasterization.Core.Rasterizers.Abstractions;
using CGRasterization.Core.ShapeHandles.Asbtractions;

namespace CGRasterization.Core.Primitives;

public class Polygon : IShape
{
    public List<Point> Vertices { get; set; }
    public Color Color { get; set; }
    public int Thickness { get; set; }
    public bool IsClosed { get; set; }
    public Polygon(List<Point> vertices, bool isClosed, Color color, int thickness)
    {
        Vertices = vertices.ToList();
        IsClosed = isClosed;
        Thickness = thickness;
        Color = color;
    }
    public double DistanceTo(Point point)
    {
        List<Line> edges = new();
        for (int i = 0; i < Vertices.Count; i++)
        {
            var vertex = Vertices[i];
            var nextVertex = Vertices[(i + 1) % Vertices.Count];
            edges.Add(new Line(vertex, nextVertex, Color,  Thickness));
        }
        return edges.Min(edge => edge.DistanceTo(point));
    }
    public void RasterizeWith(IShapeRasterizer rasterizer, PixelBuffer buffer) => rasterizer.Rasterize(this, buffer);
    public void MoveBy(int dx, int dy)
    {
        if (!IsClosed) return;
        for (int i = 0; i < Vertices.Count; i++)
        {
            Point vertex = Vertices[i];
            Vertices[i] = new Point(
                vertex.X + dx,
                vertex.Y + dy);
        }
    }
    public IShapeHandle? GetHandle(Point point, double tolerance)
    {
        throw new NotImplementedException();
    }
    public void MoveHandle(IShapeHandle handle, int dx, int dy)
    {
        throw new NotImplementedException();
    }
    public override string ToString() => $"Polygon: Vertices=[{String.Join(", ", Vertices.Select(vertex => $"({vertex.X}, {vertex.Y})"))}]";
}