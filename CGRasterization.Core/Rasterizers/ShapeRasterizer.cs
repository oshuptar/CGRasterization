using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Rasterizers;

public sealed class ShapeRasterizer : IShapeRasterizer
{
    private readonly IRasterizer<Line> _lineRasterizer = new LineRasterizer();
    private readonly IRasterizer<Circle> _circleRasterizer= new CircleRasterizer();

    public void Rasterize(Circle circle, PixelBuffer buffer)
    {
        _circleRasterizer.Rasterize(circle, buffer);
    }

    public void Rasterize(Line line, PixelBuffer buffer)
    {
        _lineRasterizer.Rasterize(line, buffer);
    }
}