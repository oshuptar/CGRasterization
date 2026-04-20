using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Rasterizers;

public class LineRasterizer : IRasterizer<Line>
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
            PutPixel(shape, buffer, x, y);
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
                    PutPixel(shape, buffer, x, y);
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
                    PutPixel(shape, buffer, x, y);
                }
            }
    }

    private void PutPixel(Line shape, PixelBuffer buffer, int x, int y)
    {
        if (x < 0 || y < 0 || x >= buffer.Width || y >= buffer.Height) return;
        int index = y*buffer.Stride + x*buffer.BytesPerPixel;
        buffer.Pixels[index] = shape.Color.R;
        buffer.Pixels[index + 1] = shape.Color.G;
        buffer.Pixels[index + 2] = shape.Color.B;
    }
}