using System.Drawing;

namespace CGRasterization.Core.Brush;

public class Brush
{
    public int Thickness { get; }
    public byte[] Pattern { get; }
    public Color Color { get; }
    public int Stride { get; }
    private byte[] BuildPattern()
    {
        byte[] pattern = new byte[Stride * Thickness];
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
                    int index = y * Stride + x * 4;
                    pattern[index] = Color.R;
                    pattern[index + 1] = Color.G;
                    pattern[index + 2] = Color.B;
                    pattern[index + 3] = Color.A;
                }
            }
        }
        return pattern;
    }
    public static int NormalizeThickness(int thickness)
    {
        if (thickness < 1) throw new ArgumentOutOfRangeException(nameof(thickness));
        return thickness % 2 == 0 ? thickness + 1 : thickness;
    }
    public Brush(Color color, int thickness)
    {
        Color = color;
        Thickness = NormalizeThickness(thickness);
        Stride = Thickness * 4;
        Pattern = BuildPattern();
    }
}