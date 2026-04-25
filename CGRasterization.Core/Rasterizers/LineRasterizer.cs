using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Rasterizers;

public class LineRasterizer : BaseRasterizer, IRasterizer<Line>
{
    public void Rasterize(Line shape, PixelBuffer buffer)
    {
        Console.WriteLine($"Rasterizing {nameof(Line)}");
        int x0 = shape.Start.X;
            int y0 = shape.Start.Y;
            int x1 = shape.End.X;
            int y1 = shape.End.Y;
            int dx = Math.Abs(shape.Dx);
            int dy = Math.Abs(shape.Dy);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int x = x0;
            int y = y0;
            Brush.Brush brush = new Brush.Brush(shape.Color, shape.Thickness);
            PutPixel(x, y, brush, buffer);
            if (dx >= dy)
            {
                int d = 2 * dy - dx;
                for (int i = 0; i < dx; i++)
                {
                    x += sx;
                    if (d < 0)
                    {
                        d += 2 * dy;
                    }
                    else
                    {
                        y += sy;
                        d += 2 * (dy - dx);
                    }
                    PutPixel(x, y, brush, buffer);
                }
            }else
            {
                int d = 2 * dx - dy;
                for (int i = 0; i < dy; i++)
                {
                    y += sy;
                    if (d < 0)
                    {
                        d += 2 * dx;
                    }
                    else
                    {
                        x += sx;
                        d += 2 * (dx - dy);
                    }
                    PutPixel(x, y,brush, buffer);
                }
            }
    }
}