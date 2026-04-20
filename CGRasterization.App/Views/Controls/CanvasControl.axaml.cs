using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not CanvasControlViewModel vm || sender is not Control canvas) return;
        e.Pointer.Capture(canvas);
        IsPressed = true;
        Start = e.GetPosition(canvas);
    }

    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        // can be used to draw temporary line
    }

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!IsPressed) return;
        if (DataContext is not CanvasControlViewModel vm || sender is not Control canvas) return;
        e.Pointer.Capture(null);
        IsPressed = false;
        End = e.GetPosition(canvas);
        vm.AddShape(new System.Drawing.Point((int)Start.X,(int) Start.Y),
            new System.Drawing.Point((int)End.X,(int) End.Y));
    }
}