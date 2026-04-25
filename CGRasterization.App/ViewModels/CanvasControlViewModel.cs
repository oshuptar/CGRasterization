using System.Collections.Generic;
using CGRasterization.App.Canvas.Enums;
using CGRasterization.App.Canvas.Tools;
using CGRasterization.App.Canvas.Tools.Abstractions;
using CGRasterization.App.ViewModels.Abstractions;
using CGRasterization.Core.Primitives.Abstractions;
using CommunityToolkit.Mvvm.Input;

namespace CGRasterization.App.ViewModels;

public class CanvasControlViewModel : ViewModelBase
{
    public Canvas.Canvas Canvas { get; set; } = new(1200, 800);

    public IShape? SelectedShape
    {
        get => field;
        set
        {
            if (field == value) return;

            field = value;
            OnPropertyChanged();
        }
    }
    public CanvasToolType SelectedToolType
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsDrawCircleMode));
            OnPropertyChanged(nameof(IsDrawLineMode));
            OnPropertyChanged(nameof(IsSelectShapeMode));
        }
    }
    public bool IsSelectShapeMode => SelectedToolType == CanvasToolType.Select;
    public bool IsDrawLineMode => SelectedToolType == CanvasToolType.Line;
    public bool IsDrawCircleMode => SelectedToolType == CanvasToolType.Circle;
    public RelayCommand SetLineDrawToolCommand { get; }
    public RelayCommand SetCircleDrawToolCommand { get; }
    public RelayCommand SetSelectShapeToolCommand { get; }
    private readonly Dictionary<CanvasToolType, ICanvasTool> _tools;
    public ICanvasTool CurrentTool => _tools[SelectedToolType];
    public CanvasControlViewModel()
    {
        SetLineDrawToolCommand = new RelayCommand(() => SetShapeType(CanvasToolType.Line), () => true);
        SetCircleDrawToolCommand = new RelayCommand(() => SetShapeType(CanvasToolType.Circle), () => true);
        SetSelectShapeToolCommand = new RelayCommand(() => SetShapeType(CanvasToolType.Select), () => true);
        _tools = new Dictionary<CanvasToolType, ICanvasTool>
        {
            [CanvasToolType.Line] = new DrawLineTool(),
            [CanvasToolType.Circle] = new DrawCircleTool(),
            [CanvasToolType.Select] = new SelectShapeTool(),
        };
        SelectedToolType = CanvasToolType.Line;
    }
    public void SetShapeType(CanvasToolType canvasToolType) => SelectedToolType = canvasToolType;
    public void AddShape(IShape shape) => Canvas.Shapes.Add(shape);
    public void SelectShape(IShape shape) => SelectedShape = shape;
    public void RemoveShape(IShape shape) => Canvas.Shapes.Remove(shape);
}