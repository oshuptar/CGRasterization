using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CGRasterization.App.Buffers;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Buffers.Enums;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Rasterizers.Factory;

namespace CGRasterization.App.Canvas;

public class Canvas : INotifyPropertyChanged
{
    private DirectBitmap Bitmap { get; set; }
    public WriteableBitmap? ImageSource
    {
        get => field;
        private set
        {
            if (ReferenceEquals(field, value))
                return;
            field = value;
            OnPropertyChanged();
        }
    }
    public int Width => Bitmap.Width;
    public int Height => Bitmap.Height;
    public ObservableCollection<Line> Lines { get; set; } = new();
    public ObservableCollection<Circle> Circles { get; set; } = new();
    public Canvas(int width, int height)
    {
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
        ImageSource = Bitmap.Bitmap;
        Lines.CollectionChanged += OnCollectionChanged<Line>;
        Circles.CollectionChanged += OnCollectionChanged<Circle>;
    }
    
    private void OnCollectionChanged<TShape>(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems is not null)
                {
                    foreach (TShape shape in e.NewItems)
                        DrawShape(shape);
                }
                break;
        }
        Bitmap.UpdateBitmap();
        InvalidateImage();
    }

    private PixelBuffer GetPixelBuffer()
    {
        return new PixelBuffer(
            Bitmap.Width,
            Bitmap.Height, 
            Bitmap.Pixels,
            Bitmap.Stride,
            Bitmap.PixelFormat == PixelFormats.Gray8 ? ColorFormat.Grayscale : ColorFormat.Rgba);
    }

    private void DrawShape<TShape>(TShape shape)
    {
        RasterizerFactory factory = new RasterizerFactory();
        var lineRasterizer = factory.GetRasterizer<TShape>();
        lineRasterizer?.Rasterize(shape, GetPixelBuffer());
    }

    private void InvalidateImage()
    {
        var current = ImageSource;
        ImageSource = null;
        ImageSource = current;
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}