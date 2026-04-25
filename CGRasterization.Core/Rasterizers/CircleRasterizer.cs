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
        Brush.Brush brush = new Brush.Brush(shape.Color, shape.Thickness);
        PutSymmetricPoints(shape, x, y, brush, buffer);
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
            PutSymmetricPoints(shape, x, y, brush, buffer);
        }
    }

    private void PutSymmetricPoints(Circle shape, int x, int y, Brush.Brush brush, PixelBuffer buffer)
    {
        int cx = shape.Center.X;
        int cy = shape.Center.Y;
        PutPixel(cx + x, cy + y, brush, buffer);
        PutPixel(cx - x, cy + y, brush, buffer);
        PutPixel(cx + x, cy - y, brush, buffer);
        PutPixel(cx - x, cy - y, brush, buffer);
        PutPixel(cx + y, cy + x, brush, buffer);
        PutPixel(cx - y, cy + x, brush, buffer);
        PutPixel(cx + y, cy - x, brush, buffer);
        PutPixel(cx - y, cy - x, brush, buffer);
    }
}