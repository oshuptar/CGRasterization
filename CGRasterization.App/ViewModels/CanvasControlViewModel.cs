using System.Collections.Generic;
using System.Drawing;
using CGRasterization.App.Canvas.Enums;
using CGRasterization.App.Canvas.Tools;
using CGRasterization.App.Canvas.Tools.Abstractions;
using CGRasterization.App.ViewModels.Abstractions;
using CGRasterization.Core.Primitives;
using CommunityToolkit.Mvvm.Input;

namespace CGRasterization.App.ViewModels;

public class CanvasControlViewModel : ViewModelBase
{
    public Canvas.Canvas Canvas { get; set; } = new(1200, 800);

    public CanvasToolType SelectedToolType
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsCircleShape));
            OnPropertyChanged(nameof(IsLineShape));
        }
    }

    public bool IsLineShape => SelectedToolType == CanvasToolType.Line;
    public bool IsCircleShape => SelectedToolType == CanvasToolType.Circle;
    public RelayCommand SetLineShapeCommand { get; }
    public RelayCommand SetCircleShapeCommand { get; }
    public RelayCommand SetMoveCommand { get; }
    private readonly Dictionary<CanvasToolType, ICanvasTool> _tools;
    public ICanvasTool CurrentTool => _tools[SelectedToolType];
    public CanvasControlViewModel()
    {
        SetLineShapeCommand = new RelayCommand(() => SetShapeType(CanvasToolType.Line), () => true);
        SetCircleShapeCommand = new RelayCommand(() => SetShapeType(CanvasToolType.Circle), () => true);
        SetMoveCommand = new RelayCommand(() => SetShapeType(CanvasToolType.Move), () => true);
        _tools = new Dictionary<CanvasToolType, ICanvasTool>
        {
            [CanvasToolType.Line] = new LineTool(),
            [CanvasToolType.Circle] = new CircleTool(),
        };
        SelectedToolType = CanvasToolType.Line;
    }

    public void SetShapeType(CanvasToolType canvasToolType)
    {
        SelectedToolType = canvasToolType;
    }
    public void AddShape(Point startPoint, Point endPoint)
    {
        if (SelectedToolType == CanvasToolType.Line)
        {
            Line newLine = new Line(startPoint, endPoint);
            Canvas.Lines.Add(newLine);
        }
        else if (SelectedToolType == CanvasToolType.Circle)
        {
            Circle newCircle = new Circle(startPoint, endPoint);
            Canvas.Circles.Add(newCircle);
        }
    }
}