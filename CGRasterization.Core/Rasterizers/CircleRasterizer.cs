using System.Drawing;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Rasterizers;

public class CircleRasterizer : BaseRasterizer, IRasterizer<Circle>
{
    public void Rasterize(Circle shape, PixelBuffer buffer)
    {
        Console.WriteLine($"Rasterizing {nameof(Circle)}");
        int x = 0;
        int y = shape.Radius;
        int d = 1 - shape.Radius; int dE = 3; int dSE = 5 - 2 * shape.Radius;
        PutSymmetricPoints(shape, x, y, buffer);
        while (x < y)
        {
            x++;
            if (d < 0)
            {
                d += dE;
                dE += 2;
                dSE += 2;
            }
            else
            {
                d += dSE;
                dE += 2;
                dSE += 4;
                y--;
            }
            PutSymmetricPoints(shape, x, y, buffer);
        }
    }

    private void PutSymmetricPoints(Circle shape, int x, int y, PixelBuffer buffer)
    {
        int cx = shape.Center.X;
        int cy = shape.Center.Y;
        Color color = shape.Color;
        PutPixel(cx + x, cy + y, color, buffer);
        PutPixel(cx - x, cy + y, color, buffer);
        PutPixel(cx + x, cy - y, color, buffer);
        PutPixel(cx - x, cy - y, color, buffer);
        PutPixel(cx + y, cy + x, color, buffer);
        PutPixel(cx - y, cy + x, color, buffer);
        PutPixel(cx + y, cy - x, color, buffer);
        PutPixel(cx - y, cy - x, color, buffer);
    }
}