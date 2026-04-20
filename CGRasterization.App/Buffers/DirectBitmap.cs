using System;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace CGRasterization.App.Buffers;
public sealed class DirectBitmap
{
    public PixelFormat PixelFormat { get; }
    public int Width { get; }
    public int Height { get; }
    public int Stride { get; }
    public Vector Dpi { get; }
    public byte[] Pixels { get; set; }
    public WriteableBitmap Bitmap { get; }
    private DirectBitmap(int width, int height, Vector dpi, PixelFormat pixelFormat)
    {
        int bytesPerPixel = pixelFormat.BitsPerPixel / 8;
        Width = width;
        Height = height;
        Dpi = dpi;
        Stride = width * bytesPerPixel;
        Pixels = new byte[Stride * height];
        PixelFormat = pixelFormat;
        Bitmap = new WriteableBitmap(
            new PixelSize(width, height),
            Dpi,
            pixelFormat,
            AlphaFormat.Unpremul
        );
    }
    
    public DirectBitmap(int width, int height, Vector dpi, PixelFormat pixelFormat, byte[] pixels)
        : this(width, height, dpi, pixelFormat)
    {
        Pixels = pixels.ToArray();
        UpdateBitmap();
    }
        
    public void UpdateBitmap()
    {
        using var fb = Bitmap.Lock();
        for (int y = 0; y < Height; y++)
        {
            IntPtr destRow = fb.Address + y * fb.RowBytes;
            Marshal.Copy(Pixels, y * Stride, destRow, Stride);
        }
    }
}
