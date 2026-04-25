using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives;

namespace CGRasterization.Core.Rasterizers.Abstractions;

public interface IShapeRasterizer
{
    void Rasterize(Line line, PixelBuffer buffer);
    void Rasterize(Circle circle, PixelBuffer buffer);
}