using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CGRasterization.App.Buffers;
using CGRasterization.Core.Abstractions;
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
        Lines.CollectionChanged += OnLinesChanged;
    }
    
    private void OnLinesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems is not null)
                {
                    foreach (Line line in e.NewItems)
                    {
                        DrawLine(line);
                    }
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                RedrawAll(Lines);
                break;

            case NotifyCollectionChangedAction.Reset:
                RedrawAll(Lines);
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

    private void DrawLine(Line line)
    {
        RasterizerFactory factory = new RasterizerFactory();
        var lineRasterizer = factory.GetLineRasterizer();
        lineRasterizer.Rasterize(line, GetPixelBuffer());
    }
    

    private void RedrawAll(IEnumerable<IDrawable> collection)
    {
        
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void InvalidateImage()
    {
        var current = ImageSource;
        ImageSource = null;
        ImageSource = current;
    }
}