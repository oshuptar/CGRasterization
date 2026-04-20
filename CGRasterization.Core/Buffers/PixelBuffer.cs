using CGRasterization.Core.Buffers.Enums;

namespace CGRasterization.Core.Buffers;

public class PixelBuffer
{
    public int Width { get; }
    public int Height { get; }
    public byte[] Pixels { get; }
    public int Stride { get; } // Width * BytesPerPixel
    public int BytesPerPixel => Stride / Width;
    public ColorFormat ColorFormat { get; private set; }
    public PixelBuffer(int width, int height, byte[] pixels, int stride, ColorFormat colorFormat)
    {
        Width = width;
        Height = height;
        Pixels = pixels;
        ColorFormat = colorFormat;
        Stride = stride;
    }
}