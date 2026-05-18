using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Rasterizers;

public sealed class ShapeRasterizer : IShapeRasterizer
{
    private readonly LineRasterizer.LineRasterizer _lineRasterizer = new();
    private readonly IRasterizer<Circle> _circleRasterizer = new CircleRasterizer();
    private readonly IRasterizer<Polygon> _polygonRasterizer;
    private readonly IRasterizer<Rectangle> _rectangleRasterizer;
    public LineRasterizationMode LineRasterizationMode
    {
        get => _lineRasterizer.Mode;
        set => _lineRasterizer.Mode = value;
    }
    public ShapeRasterizer()
    {
        _polygonRasterizer = new PolygonRasterizer(_lineRasterizer);
        _rectangleRasterizer = new RectangleRasterizer(_lineRasterizer);
    }
    public void Rasterize(Circle circle, PixelBuffer buffer) => _circleRasterizer.Rasterize(circle, buffer);
    public void Rasterize(Polygon polygon, PixelBuffer buffer) => _polygonRasterizer.Rasterize(polygon, buffer);
    public void Rasterize(Line line, PixelBuffer buffer) => _lineRasterizer.Rasterize(line, buffer);
    public void Rasterize(Rectangle rectangle, PixelBuffer buffer) => _rectangleRasterizer.Rasterize(rectangle, buffer);
}
