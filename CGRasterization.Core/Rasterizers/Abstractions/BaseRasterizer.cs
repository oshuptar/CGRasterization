using System.Drawing;
using CGRasterization.Core.Buffers;

namespace CGRasterization.Core.Rasterizers.Abstractions;

public abstract class BaseRasterizer
{
    protected void PutPixel(int x, int y, Brush.Brush brush, PixelBuffer buffer)
    {
        if (x < 0 || y < 0 || x >= buffer.Width || y >= buffer.Height) return;
        int brushRadius = brush.Thickness / 2;
        int i = 0, j;
        for (int cy = y - brushRadius; cy <= y + brushRadius; cy++, i++)
        {
            j = 0;
            for (int cx = x - brushRadius; cx <= x + brushRadius; cx++, j++)
            {
                if (cx < 0 || cy < 0 || cx >= buffer.Width || cy >= buffer.Height) continue;
                int bufferIndex = cy*buffer.Stride + cx*buffer.BytesPerPixel;
                int patternIndex = i * brush.Stride + j * 4;
                
                // if this pixel is outside the brush shape, do nothing
                if (brush.Pattern[patternIndex + 3] == 0)
                    continue;
                buffer.Pixels[bufferIndex] = brush.Pattern[patternIndex];
                buffer.Pixels[bufferIndex + 1] = brush.Pattern[patternIndex + 1];
                buffer.Pixels[bufferIndex + 2] = brush.Pattern[patternIndex + 2];
                buffer.Pixels[bufferIndex + 3] = brush.Pattern[patternIndex + 3];
            }
        }
    }
}