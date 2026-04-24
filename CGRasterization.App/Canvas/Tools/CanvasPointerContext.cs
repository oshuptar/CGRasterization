using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CGRasterization.App.ViewModels;

namespace CGRasterization.App.Canvas.Tools;

public sealed class CanvasPointerContext
{
    public required Point Position { get; init; }
    public required CanvasControlViewModel ViewModel { get; init; }
    public required IPointer Pointer { get; init; }
    public required Control Canvas { get; init; }
}