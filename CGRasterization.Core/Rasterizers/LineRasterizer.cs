using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Rasterizers;

public class LineRasterizer : IRasterizer<Line>
{
    public void Rasterize(Line shape, PixelBuffer buffer)
    {
        // Implement the algorithm here
        int x0 = shape.Start.X;
            int y0 = shape.Start.Y;
            int x1 = shape.End.X;
            int y1 = shape.End.Y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);

            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;

            int x = x0;
            int y = y0;

            int index = y*buffer.Stride + x*buffer.BytesPerPixel;
            buffer.Pixels[index] = shape.Color.R;
            buffer.Pixels[index + 1] = shape.Color.G;
            buffer.Pixels[index + 2] = shape.Color.B;
        
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
                    index = y*buffer.Stride + x*buffer.BytesPerPixel;
                    buffer.Pixels[index] = shape.Color.R;
                    buffer.Pixels[index + 1] = shape.Color.G;
                    buffer.Pixels[index + 2] = shape.Color.B;
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
                    index = y*buffer.Stride + x*buffer.BytesPerPixel;
                    buffer.Pixels[index] = shape.Color.R;
                    buffer.Pixels[index + 1] = shape.Color.G;
                    buffer.Pixels[index + 2] = shape.Color.B;
                }
            }
    }
}