using System.Drawing;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Rasterizers.Abstractions;
using Rectangle = CGRasterization.Core.Primitives.Rectangle;

namespace CGRasterization.Core.Rasterizers;

public class RectangleRasterizer : BaseRasterizer, IRasterizer<Rectangle>
{
    private readonly IRasterizer<Line> _lineRasterizer;

    public RectangleRasterizer(IRasterizer<Line> lineRasterizer)
    {
        _lineRasterizer = lineRasterizer;
    }

    public void Rasterize(Rectangle shape, PixelBuffer buffer)
    {
        DrawEdge(shape.TopLeft, shape.TopRight, shape, buffer);
        DrawEdge(shape.TopRight, shape.BottomRight, shape, buffer);
        DrawEdge(shape.BottomRight, shape.BottomLeft, shape, buffer);
        DrawEdge(shape.BottomLeft, shape.TopLeft, shape, buffer);
    }

    private void DrawEdge(Point start, Point end, Rectangle shape, PixelBuffer buffer)
    {
        Line line = new Line(start, end, shape.Color, shape.Thickness);
        _lineRasterizer.Rasterize(line, buffer);
    }
}
