using System.Drawing;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives.Abstractions;
using CGRasterization.Core.Rasterizers.Abstractions;
using CGRasterization.Core.ShapeHandles;
using CGRasterization.Core.ShapeHandles.Asbtractions;
using CGRasterization.Core.Utilities;

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
    private IEnumerable<IShapeHandle> GetVertexHandles()
    {
        for (int i = 0; i < Vertices.Count; i++)
        {
            int index = i;
            yield return new ShapeHandle(
                getPosition: () => Vertices[index],
                setPosition: point => Vertices[index] = point);
        }
    }
    private IEnumerable<(int StartIndex, int EndIndex, Point Start, Point End)> GetEdges()
    {
        if (!IsClosed || Vertices.Count < 2) yield break;
        for (int i = 0; i < Vertices.Count; i++)
        {
            int startIndex = i;
            int endIndex = (i + 1) % Vertices.Count;
            yield return (startIndex, endIndex,
                Vertices[startIndex],
                Vertices[endIndex]);
        }
    }
    
    public IShapeHandle? GetHandle(Point point, double tolerance)
    {
        IShapeHandle? vertexHandle = GetVertexHandles()
            .Select(handle => new
            {
                Handle = handle,
                Distance = GeometryUtilities.Distance(point, handle.Position)
            })
            .Where(x => x.Distance <= tolerance)
            .OrderBy(x => x.Distance)
            .Select(x => x.Handle)
            .FirstOrDefault();
        
        if (vertexHandle is not null) return vertexHandle;
        return GetEdges()
            .Select(edge => new
            {
                Edge = edge,
                Distance = GeometryUtilities.DistanceToSegment(point, edge.Start, edge.End)
            })
            .Where(x => x.Distance <= tolerance)
            .OrderBy(x => x.Distance)
            .Select(x => CreateEdgeDragHandle(x.Edge.StartIndex, x.Edge.EndIndex, point))
            .FirstOrDefault();
    }
    private IShapeHandle CreateEdgeDragHandle(int startIndex, int endIndex, Point grabbedPoint)
    {
        Point handlePosition = grabbedPoint;
        return new ShapeHandle(
            getPosition: () => handlePosition,
            setPosition: newPosition =>
            {
                int dx = newPosition.X - handlePosition.X;
                int dy = newPosition.Y - handlePosition.Y;
                Point a = Vertices[startIndex];
                Point b = Vertices[endIndex];
                Vertices[startIndex] = new Point(a.X + dx, a.Y + dy);
                Vertices[endIndex] = new Point(b.X + dx, b.Y + dy);
                handlePosition = newPosition;
            });
    }
    public void MoveHandle(IShapeHandle handle, int dx, int dy) => handle.MoveBy(dx, dy);
    public override string ToString() => $"Polygon: Vertices=[{String.Join(", ", Vertices.Select(vertex => $"({vertex.X}, {vertex.Y})"))}]";
}