using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CGRasterization.App.Buffers;
using CGRasterization.Core.Buffers;
using CGRasterization.Core.Buffers.Enums;
using CGRasterization.Core.Primitives.Abstractions;
using CGRasterization.Core.Rasterizers;
using CGRasterization.Core.Rasterizers.Abstractions;
using Brush = CGRasterization.Core.Brush.Brush;
using Color = System.Drawing.Color;

namespace CGRasterization.App.Canvas;

public class Canvas : INotifyPropertyChanged
{
    private readonly IShapeRasterizer _shapeRasterizer = new ShapeRasterizer();
    public Brush Brush { get; set; }
    public int BrushThickness
    {
        get;
        set
        {
            int normalized = value % 2 == 0 ? value + 1 : value;
            if (field == normalized) return;
            field = normalized;
            Brush.Thickness = normalized;
            OnPropertyChanged();
        }
    } = 1;

    public Avalonia.Media.Color BrushColorPicker
    {
        get;
        set
        {
            field = value;
            Brush.Color = Color.FromArgb(value.A, value.R, value.G, value.B);
            OnPropertyChanged();
            OnPropertyChanged(nameof(BrushColorName)); 
        }
    } = Colors.Black;
    public string BrushColorName => BrushColorPicker.ToString();
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
    public IShape? PreviewShape { get; set; }
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
        Brush = new(Color.FromArgb(BrushColorPicker.A, BrushColorPicker.R, BrushColorPicker.G, BrushColorPicker.B) , BrushThickness);
        Bitmap = new DirectBitmap(width, height, new Vector(96, 96), PixelFormat.Rgba8888, bytes);
        Bitmap.UpdateBitmap();
        ImageSource = Bitmap.Bitmap;
        Shapes.CollectionChanged += OnCollectionChanged;
    }
    public void SetPreviewShape(IShape? shape)
    {
        PreviewShape = shape;
        RedrawShapes();
    }
    public void RedrawShapes()
    {
        Console.WriteLine("Redrawing Shapes");
        PixelBuffer buffer = GetPixelBuffer();
        Clear();
        foreach (IShape shape in Shapes)
            DrawShape(shape, buffer);
        if(PreviewShape is not null)
            DrawShape(PreviewShape, buffer);
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