using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Rasterizers.LineRasterizer;

public class LineRasterizer : IRasterizer<Line>
{
    private readonly IRasterizer<Line> _bresenhamRasterizer = new BresenhamRasterizer();
    private readonly IRasterizer<Line> _guptaSproullRasterizer = new GuptaSproullRasterizer();
    public LineRasterizationMode Mode { get; set; } = LineRasterizationMode.Bresenham;
    public void Rasterize(Line line, PixelBuffer buffer)
    {
        IRasterizer<Line> rasterizer = Mode switch
        {
            LineRasterizationMode.Bresenham => _bresenhamRasterizer,
            LineRasterizationMode.GuptaSproull => _guptaSproullRasterizer,
            _ => _bresenhamRasterizer
        };
        rasterizer.Rasterize(line, buffer);
    }
}