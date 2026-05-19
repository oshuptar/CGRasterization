namespace CGRasterization.Core.ImagePattern;

public class ImagePattern
{
    public byte[] Pixels { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public ImagePattern(byte[] pixels, int width, int height)
    {
        Pixels = pixels;
        Width = width;
        Height = height;
    }
}