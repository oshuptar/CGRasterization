using System.Drawing;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives.Abstractions;
using CGRasterization.Core.Rasterizers.Abstractions;
using CGRasterization.Core.ShapeHandles;
using CGRasterization.Core.ShapeHandles.Asbtractions;
using CGRasterization.Core.Utilities;

namespace CGRasterization.Core.Primitives;

public class Rectangle : IShape
{
    public Point TopLeft { get; set; }
    public Point BottomRight { get; set; }
    public int Thickness { get; set; }
    public Color Color { get; set; }
    public Point TopRight => new(BottomRight.X, TopLeft.Y);
    public Point BottomLeft => new(TopLeft.X, BottomRight.Y);
    public int Width => BottomRight.X - TopLeft.X;
    public int Height => BottomRight.Y - TopLeft.Y;

    public Rectangle(Point corner1, Point corner2, Color color, int thickness)
    {
        TopLeft = new Point(Math.Min(corner1.X, corner2.X), Math.Min(corner1.Y, corner2.Y));
        BottomRight = new Point(Math.Max(corner1.X, corner2.X), Math.Max(corner1.Y, corner2.Y));
        Color = color;
        Thickness = thickness;
    }

    public void RasterizeWith(IShapeRasterizer rasterizer, PixelBuffer buffer) =>
        rasterizer.Rasterize(this, buffer);

    public double DistanceTo(Point point)
    {
        double minEdgeDistance = new[]
        {
            GeometryUtilities.DistanceToSegment(point, TopLeft, TopRight),
            GeometryUtilities.DistanceToSegment(point, TopRight, BottomRight),
            GeometryUtilities.DistanceToSegment(point, BottomRight, BottomLeft),
            GeometryUtilities.DistanceToSegment(point, BottomLeft, TopLeft),
        }.Min();
        return Math.Max(minEdgeDistance - Thickness / 2, 0);
    }

    public void MoveBy(int dx, int dy)
    {
        TopLeft = new Point(TopLeft.X + dx, TopLeft.Y + dy);
        BottomRight = new Point(BottomRight.X + dx, BottomRight.Y + dy);
    }

    private IEnumerable<IShapeHandle> GetVertexHandles()
    {
        yield return new ShapeHandle(
            getPosition: () => TopLeft,
            setPosition: p => TopLeft = p);
        yield return new ShapeHandle(
            getPosition: () => new Point(BottomRight.X, TopLeft.Y),
            setPosition: p => { BottomRight = new Point(p.X, BottomRight.Y); TopLeft = new Point(TopLeft.X, p.Y); });
        yield return new ShapeHandle(
            getPosition: () => BottomRight,
            setPosition: p => BottomRight = p);
        yield return new ShapeHandle(
            getPosition: () => new Point(TopLeft.X, BottomRight.Y),
            setPosition: p => { TopLeft = new Point(p.X, TopLeft.Y); BottomRight = new Point(BottomRight.X, p.Y); });
    }
    private IEnumerable<(Point Start, Point End, Action<int, int> OnDrag)> GetEdges()
    {
        yield return (TopLeft, new Point(BottomRight.X, TopLeft.Y),
            (_, dy) => TopLeft = new Point(TopLeft.X, TopLeft.Y + dy));
        yield return (new Point(BottomRight.X, TopLeft.Y), BottomRight,
            (dx, _) => BottomRight = new Point(BottomRight.X + dx, BottomRight.Y));
        yield return (BottomRight, new Point(TopLeft.X, BottomRight.Y),
            (_, dy) => BottomRight = new Point(BottomRight.X, BottomRight.Y + dy));
        yield return (new Point(TopLeft.X, BottomRight.Y), TopLeft,
            (dx, _) => TopLeft = new Point(TopLeft.X + dx, TopLeft.Y));
    }
    private IShapeHandle CreateEdgeDragHandle(Action<int, int> onDrag, Point grabbedPoint)
    {
        Point handlePosition = grabbedPoint;
        return new ShapeHandle(
            getPosition: () => handlePosition,
            setPosition: newPosition =>
            {
                int dx = newPosition.X - handlePosition.X;
                int dy = newPosition.Y - handlePosition.Y;
                onDrag(dx, dy);
                handlePosition = newPosition;
            }); 
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
            .Select(x => CreateEdgeDragHandle(x.Edge.OnDrag, point))
            .FirstOrDefault();
    }
    public void MoveHandle(IShapeHandle handle, int dx, int dy) => handle.MoveBy(dx, dy);
    public override string ToString() => $"Rectangle: TopLeft=({TopLeft.X}, {TopLeft.Y})\nBottomRight=({BottomRight.X}, {BottomRight.Y})";
}
