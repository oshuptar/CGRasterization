using System.Drawing;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Rasterizers;

public class PolygonRasterizer : BaseRasterizer, IRasterizer<Polygon>
{
    private readonly IRasterizer<Line> _lineRasterizer;
    public PolygonRasterizer(IRasterizer<Line> lineRasterizer)
    {
        _lineRasterizer = lineRasterizer;
    }
    public void Rasterize(Polygon shape, PixelBuffer buffer)
    {
        for (int i = 0; i < shape.Vertices.Count - 1; i++)
        {
            Point currentPoint = shape.Vertices[i];
            Point nextPoint = shape.Vertices[i + 1];
            DrawEdge(currentPoint, nextPoint, shape, buffer);
        }
        if (shape.IsClosed && shape.Vertices.Count >= 3)
        {
            Point lastPoint = shape.Vertices[^1];
            Point firstPoint = shape.Vertices[0];
            DrawEdge(lastPoint, firstPoint, shape, buffer);
        }
        //DrawVertices(shape, buffer);
    }
    private void DrawEdge(Point start, Point end, Polygon shape, PixelBuffer buffer)
    {
        Line line = new Line(start, end, shape.Color, shape.Thickness);
        _lineRasterizer.Rasterize(line, buffer);
    }
    
    // private void DrawVertices(Polygon shape, PixelBuffer buffer)
    // {
    //     Brush.Brush brush = new Brush.Brush(shape.Color, shape.Thickness);
    //     foreach (Point vertex in shape.Vertices)
    //         PutPixel(vertex.X, vertex.Y, brush, buffer);
    // }
}