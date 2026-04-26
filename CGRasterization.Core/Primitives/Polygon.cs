using System.Drawing;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives.Abstractions;
using CGRasterization.Core.Rasterizers.Abstractions;
using CGRasterization.Core.ShapeHandles;
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
        if (!IsClosed || Vertices.Count < 2)
            return double.MaxValue;
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
    private IEnumerable<IShapeHandle> GetHandles()
    {
        for (int i = 0; i < Vertices.Count; i++)
        {
            int index = i;
            yield return new ShapeHandle(
                getPosition: () => Vertices[index],
                setPosition: point => Vertices[index] = point);
        }
    }
    public IShapeHandle? GetHandle(Point point, double tolerance)
    {
        return GetHandles()
            .Select(handle => new
            {
                Handle = handle,
                Distance = Math.Sqrt(Math.Pow(point.X - handle.Position.X, 2) + Math.Pow(point.Y - handle.Position.Y, 2))
            })
            .Where(x => x.Distance <= tolerance)
            .OrderBy(x => x.Distance)
            .Select(x => x.Handle)
            .FirstOrDefault();
    }
    public void MoveHandle(IShapeHandle handle, int dx, int dy) => handle.MoveBy(dx, dy);
    public override string ToString() => $"Polygon: Vertices=[{String.Join(", ", Vertices.Select(vertex => $"({vertex.X}, {vertex.Y})"))}]";
}