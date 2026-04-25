using System.Drawing;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Abstractions;

public interface IDrawable
{
    void RasterizeWith(IShapeRasterizer rasterizer, PixelBuffer buffer);
    public Color Color { get; set; }
    public int Thickness { get; set; }
}