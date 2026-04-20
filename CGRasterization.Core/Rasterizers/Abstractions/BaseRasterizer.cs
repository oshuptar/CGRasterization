using System.Drawing;
using CGRasterization.Core.Buffers;

namespace CGRasterization.Core.Rasterizers.Abstractions;

public abstract class BaseRasterizer
{
    protected void PutPixel(int x, int y, Color color, PixelBuffer buffer)
    {
        if (x < 0 || y < 0 || x >= buffer.Width || y >= buffer.Height) return;
        int index = y*buffer.Stride + x*buffer.BytesPerPixel;
        buffer.Pixels[index] = color.R;
        buffer.Pixels[index + 1] = color.G;
        buffer.Pixels[index + 2] = color.B;
    }
}