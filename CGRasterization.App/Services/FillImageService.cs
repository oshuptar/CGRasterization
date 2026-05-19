using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CGRasterization.App.Services.Asbtractions;
using CGRasterization.Core.ImagePattern;

namespace CGRasterization.App.Services;

public sealed class FillImageService : IFillImageService
{
    public ImagePattern? Load(Stream stream)
    {
        var bitmap = new Bitmap(stream);

        int width = bitmap.PixelSize.Width;
        int height = bitmap.PixelSize.Height;
        int stride = width * 4;
        int bufferSize = stride * height;

        byte[] pixels = new byte[bufferSize];

        // WriteableBitmap allocates its pixel buffer in unmanaged memory with Rgba8888 layout.
        // By specifying Rgba8888 here copyPixels will convert the source format so we always receive RGBA bytes regardless of the source imageformat   
        var writable = new WriteableBitmap(
            new PixelSize(width, height),
            new Vector(96, 96),
            PixelFormat.Rgba8888,
            AlphaFormat.Unpremul);

        using (var framebuffer = writable.Lock())
        {
            // framebuffer.Address is a raw pointer into unmanaged memory owned by WriteableBitmap.
            // copyPixels writes Rgba8888 pixels directly into unmanaged buffer.
            bitmap.CopyPixels(framebuffer);

            // transfers bytes from the unmanaged framebuffer into the managed pixels array
            Marshal.Copy(framebuffer.Address, pixels, 0, bufferSize);
        }
        return new (pixels, width, height);
    }
}
