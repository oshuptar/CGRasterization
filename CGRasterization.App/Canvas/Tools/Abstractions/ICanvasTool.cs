namespace CGRasterization.App.Canvas.Tools.Abstractions;

public interface ICanvasTool
{ 
    void OnPointerPressed(CanvasPointerContext context);
    void OnPointerMoved(CanvasPointerContext context);
    void OnPointerReleased(CanvasPointerContext context);
}