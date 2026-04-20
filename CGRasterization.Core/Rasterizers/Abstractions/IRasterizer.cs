using System.Drawing;
using CGRasterization.Core.Buffers;

namespace CGRasterization.Core.Rasterizers.Abstractions;

public interface IRasterizer<TShape>
{
    public void Rasterize(TShape  shape, PixelBuffer buffer);
}