using System.Drawing;

namespace CGRasterization.Core.Brush;

public class Brush
{
    public int Thickness {get; set;}
    public byte[] Pattern { get; set; }
    public Color  Color { get; set; }
    public int Stride { get; set; }
    public Brush(int thickness, Color color)
    {
        if (thickness < 1)
            throw new ArgumentOutOfRangeException(nameof(thickness));

        Thickness = thickness % 2 == 0 ? thickness - 1 : thickness;
        int stride = Thickness * 4;
        Stride = stride;
        Pattern = new byte[Stride * Thickness];
        int centerX = Thickness / 2;
        int centerY = Thickness / 2;
        int radius = Thickness / 2;
        for (int y = 0; y < Thickness; y++)
        {
            int centeredY = y - centerY;
            for (int x = 0; x < Thickness; x++)
            {
                int centeredX = x - centerX;
                if (centeredX * centeredX + centeredY * centeredY <= radius * radius)
                {
                    int index = y * stride + x * 4;
                    Pattern[index]     = color.R;
                    Pattern[index + 1] = color.G;
                    Pattern[index + 2] = color.B;
                    Pattern[index + 3] = color.A;
                }
            }
        }
    }
}