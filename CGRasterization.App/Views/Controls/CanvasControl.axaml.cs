using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CGRasterization.App.Canvas.Tools;
using CGRasterization.App.ViewModels;


namespace CGRasterization.App.Views.Controls;

public partial class CanvasControl : UserControl
{
    public Point Start { get; set; }
    public Point End { get; set; }
    public bool IsPressed { get; set; } = false;
    public CanvasControl()
    {
        InitializeComponent();
    }
    
    private CanvasPointerContext? CreateContext(object? sender, PointerEventArgs e)
    {
        if (DataContext is not CanvasControlViewModel vm)
            return null;
        if (sender is not Control canvas)
            return null;
        return new CanvasPointerContext
        {
            Position = e.GetPosition(canvas),
            ViewModel = vm,
            Pointer = e.Pointer,
            Canvas = canvas
        };
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var context = CreateContext(sender, e);
        if (context is null) return;

        e.Pointer.Capture(context.Canvas);
        context.ViewModel.CurrentTool?.OnPointerPressed(context);
    }

    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var context = CreateContext(sender, e);
        if (context is null) return;

        context.ViewModel.CurrentTool?.OnPointerMoved(context);
    }

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var context = CreateContext(sender, e);
        if (context is null) return;
        context.ViewModel.CurrentTool?.OnPointerReleased(context);
        e.Pointer.Capture(null);
    }
}