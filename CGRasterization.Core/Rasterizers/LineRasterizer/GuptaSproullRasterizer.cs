using System.Drawing;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Rasterizers.LineRasterizer;

public class GuptaSproullRasterizer : BaseRasterizer, IRasterizer<Line>
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
        
        if (dx == 0 && dy == 0)
        {
            Brush.Brush brush = new Brush.Brush(shape.Color, shape.Thickness);
            PutPixel(x0, y0, brush, buffer);
            return;
        }
        
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        double invDenom = 1.0 / (2.0 * Math.Sqrt(dx * dx + dy * dy));
        double twoDxInvDenom = 2.0 * dx * invDenom;
        double twoDyInvDenom = 2.0 * dy * invDenom;
        int x = x0; int y = y0;
        if (dx >= dy)
        {
            int d = 2 * dy - dx;
            double twoVDx = 0;
            double signedDistance = twoVDx * invDenom;
            IntensifyPixel(x, y, shape.Thickness, Math.Abs(signedDistance), shape.Color, buffer);
            for (int i = 1; IntensifyPixel(x, y + sy * i, shape.Thickness, Math.Abs(i * twoDxInvDenom - signedDistance), shape.Color, buffer) != 0; i++)
            {
            }

            for (int i = 1; IntensifyPixel(x, y - sy * i, shape.Thickness, Math.Abs(i * twoDxInvDenom + signedDistance), shape.Color, buffer) != 0; i++)
            {
            }
            for (int j = 0; j < dx; j++)
            {
                x += sx;
                if (d < 0)
                {
                    twoVDx = d + dx;
                    d += 2 * dy;
                }
                else
                {
                    twoVDx = d - dx;
                    y += sy;
                    d += 2 * (dy - dx);
                }
                signedDistance = twoVDx * invDenom;
                IntensifyPixel(x, y, shape.Thickness, Math.Abs(signedDistance), shape.Color, buffer);
                for (int i = 1; IntensifyPixel(x, y + sy * i, shape.Thickness, Math.Abs(i * twoDxInvDenom - signedDistance), shape.Color, buffer) != 0; i++)
                {
                }
                for (int i = 1; IntensifyPixel(x, y - sy*i, shape.Thickness, Math.Abs(i * twoDxInvDenom + signedDistance), shape.Color, buffer) != 0; i++)
                {
                }
            }
        }
        else
        {
            int d = 2 * dx - dy;
            double twoUDy = 0;
            double signedDistance = twoUDy * invDenom;
            IntensifyPixel(x, y, shape.Thickness, Math.Abs(signedDistance), shape.Color, buffer);
            for (int i = 1; IntensifyPixel(x + sx * i, y, shape.Thickness, Math.Abs(i * twoDyInvDenom - signedDistance), shape.Color, buffer) != 0; i++)
            {
            }
            for (int i = 1; IntensifyPixel(x - sx * i, y, shape.Thickness, Math.Abs(i * twoDyInvDenom + signedDistance), shape.Color, buffer) != 0; i++)
            {
            }
            for (int j = 0; j < dy; j++)
            {
                y += sy;
                if (d < 0)
                {
                    twoUDy = d + dy;
                    d += 2 * dx;
                }
                else
                {
                    twoUDy = d - dy;
                    x += sx;
                    d += 2 * (dx - dy);
                }
                signedDistance = twoUDy * invDenom;
                IntensifyPixel(x, y, shape.Thickness, Math.Abs(signedDistance), shape.Color, buffer);
                for (int i = 1;
                     IntensifyPixel(x + sx * i, y, shape.Thickness, Math.Abs(i * twoDyInvDenom - signedDistance), shape.Color, buffer) != 0;
                     i++)
                {
                }
                for (int i = 1;
                     IntensifyPixel(x - sx* i, y, shape.Thickness, Math.Abs(i * twoDyInvDenom + signedDistance), shape.Color, buffer) != 0;
                     i++)
                {
                }
            }
        }
        Brush.Brush vertexBrush = new Brush.Brush(shape.Color, shape.Thickness);
        PutPixel(x0, y0, vertexBrush, buffer);
        PutPixel(x1, y1, vertexBrush, buffer);
    }

    private int IntensifyPixel(int x, int y, double thickness, double distance, Color requestedColor, PixelBuffer buffer)
    {
        double radius = 0.5;
        double coverage = Coverage(thickness, distance, radius);
        if (coverage > 0) PutPixelCoverage(x, y, requestedColor, coverage, buffer);
        return coverage >= 1.0 ? 1 : 0;
    }

    private double Coverage(double thickness, double distance, double radius)
    {
        Func<double, double, double> coverage = (distance, radius) =>
        {
            distance = Math.Clamp(distance, 0.0, radius);
            if (distance >= radius) return 0;
            return Math.Acos(distance / radius) / Math.PI -
                   (distance * Math.Sqrt(radius * radius - distance * distance)) / (Math.PI * radius * radius);
        };
        double halfWidth = thickness / 2.0;
        // Line thicker than the pixel case
        if (halfWidth <= distance)
            return coverage(distance - halfWidth, radius);
        if (halfWidth >= distance && distance >= 0)
            return 1 - coverage(halfWidth - distance, radius);
        return 0;
    }
    
    private void PutPixelCoverage(int x, int y, Color color, double coverage, PixelBuffer buffer)
    {
        if (x < 0 || y < 0 || x >= buffer.Width || y >= buffer.Height) return;
        
        coverage = Math.Clamp(coverage, 0.0, 1.0);
        Color background = Color.White;
        byte newR = (byte)(background.R * (1.0 - coverage) + color.R * coverage);
        byte newG = (byte)(background.G * (1.0 - coverage) + color.G * coverage);
        byte newB = (byte)(background.B * (1.0 - coverage) + color.B * coverage);
        int index = y * buffer.Stride + x * buffer.BytesPerPixel;
        byte oldR = buffer.Pixels[index];
        byte oldG = buffer.Pixels[index + 1];
        byte oldB = buffer.Pixels[index + 2];
        // Since background is white and line colors are usually darker than white keep the draker color
        Color resultingColor = Color.FromArgb(
            255,
            Math.Min(oldR, newR),
            Math.Min(oldG, newG),
            Math.Min(oldB, newB)
        );
        Brush.Brush brush = new Brush.Brush(resultingColor, 1);
        PutPixel(x, y, brush, buffer);
    }
}