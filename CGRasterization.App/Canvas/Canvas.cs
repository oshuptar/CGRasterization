using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CGRasterization.App.Buffers;

namespace CGRasterization.App.Canvas;

public class Canvas
{
    private DirectBitmap Bitmap { get; set; }
    public WriteableBitmap ImageSource => Bitmap.Bitmap;
    public int Width { get; set; }
    public int Height { get; set; }

    public Canvas(int width, int height)
    {
        Width = width;
        Height = height;
        byte[] bytes = new byte[width * height * 4];
        for (int i = 0; i < bytes.Length; i += 4)
        {
            bytes[i] = 255;
            bytes[i + 1] = 255;
            bytes[i + 2] = 255;
            bytes[i + 3] = 255; 
        }
        Bitmap = new DirectBitmap(width, height, new Vector(96, 96), PixelFormat.Rgba8888, bytes);
        Bitmap.UpdateBitmap();
    }
}