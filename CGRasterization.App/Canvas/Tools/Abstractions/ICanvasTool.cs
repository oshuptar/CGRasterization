namespace CGRasterization.App.Canvas.Tools.Abstractions;

public interface ICanvasTool
{ 
    public void OnPointerPressed(CanvasPointerContext context);
    public void OnPointerMoved(CanvasPointerContext context);
    public void OnPointerReleased(CanvasPointerContext context);
    public void Cancel();
}