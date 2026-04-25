using System;
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
using CGRasterization.Core.Primitives.Abstractions;
using CGRasterization.Core.Rasterizers;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.App.Canvas;

public class Canvas : INotifyPropertyChanged
{
    private readonly IShapeRasterizer _shapeRasterizer = new ShapeRasterizer();
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
    public ObservableCollection<IShape> Shapes { get; } = new();
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
        Shapes.CollectionChanged += OnCollectionChanged;
    }
    public void RedrawShapes()
    {
        Console.WriteLine("Redrawing Shapes");
        PixelBuffer buffer = GetPixelBuffer();
        Clear();
        foreach (IShape shape in Shapes)
        {
            DrawShape(shape, buffer);
        }
        Bitmap.UpdateBitmap();
        InvalidateImage();
    }
    
    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        PixelBuffer buffer = GetPixelBuffer();
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems is not null)
                {
                    foreach (IShape shape in e.NewItems)
                        DrawShape(shape, buffer);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                RedrawShapes();
                break;
            default:
                RedrawShapes();
                break;
        }
        Bitmap.UpdateBitmap();
        InvalidateImage();
    }

    private PixelBuffer GetPixelBuffer() => new(
            Bitmap.Width,
            Bitmap.Height, 
            Bitmap.Pixels,
            Bitmap.Stride,
            Bitmap.PixelFormat == PixelFormats.Gray8 ? ColorFormat.Grayscale : ColorFormat.Rgba);
    private void DrawShape(IShape shape, PixelBuffer buffer) => shape.RasterizeWith(_shapeRasterizer, buffer);
    private void InvalidateImage()
    {
        var current = ImageSource;
        ImageSource = null;
        ImageSource = current;
    }
    private void Clear()
    {
        Console.WriteLine("Clearing Canvas");
        for (int i = 0; i < Bitmap.Pixels.Length; i += 4)
        {
            Bitmap.Pixels[i] = 255;
            Bitmap.Pixels[i + 1] = 255;
            Bitmap.Pixels[i + 2] = 255;
            Bitmap.Pixels[i + 3] = 255;
        }
    }

    public void ClearCanvas()
    {
        Clear();
        Bitmap.UpdateBitmap();
        InvalidateImage();
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}