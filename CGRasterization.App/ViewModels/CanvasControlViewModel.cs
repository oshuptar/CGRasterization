using System;
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
            OnPropertyChanged(nameof(IsSelectedShape));
            OnPropertyChanged(nameof(SelectedShapeControlsOpacity));
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
    public double SelectedShapeControlsOpacity => IsSelectedShape ? 1.0 : 0.0;
    public bool IsSelectedShape => SelectedShape != null;
    public bool IsSelectShapeMode => SelectedToolType == CanvasToolType.SelectShape;
    public bool IsDrawLineMode => SelectedToolType == CanvasToolType.DrawLine;
    public bool IsDrawCircleMode => SelectedToolType == CanvasToolType.DrawCircle;
    public RelayCommand SetLineDrawToolCommand { get; }
    public RelayCommand SetCircleDrawToolCommand { get; }
    public RelayCommand SetSelectShapeToolCommand { get; }
    public RelayCommand RemoveShapeCommand { get; }
    public RelayCommand MoveShapeCommand { get; }
    private readonly Dictionary<CanvasToolType, ICanvasTool> _tools;
    public ICanvasTool CurrentTool => _tools[SelectedToolType];
    public CanvasControlViewModel()
    {
        SetLineDrawToolCommand = new RelayCommand(() => SetToolMode(CanvasToolType.DrawLine), () => true);
        SetCircleDrawToolCommand = new RelayCommand(() => SetToolMode(CanvasToolType.DrawCircle), () => true);
        SetSelectShapeToolCommand = new RelayCommand(() => SetToolMode(CanvasToolType.SelectShape), () => true);
        RemoveShapeCommand = new RelayCommand(() => RemoveShape(SelectedShape), () => true);
        MoveShapeCommand = new RelayCommand(() => SetToolMode(CanvasToolType.MoveShape), () => true);
        _tools = new Dictionary<CanvasToolType, ICanvasTool>
        {
            [CanvasToolType.DrawLine] = new DrawLineTool(),
            [CanvasToolType.DrawCircle] = new DrawCircleTool(),
            [CanvasToolType.SelectShape] = new SelectShapeTool(),
        };
        SelectedToolType = CanvasToolType.DrawLine;
    }
    public void SetToolMode(CanvasToolType canvasToolType)
    {
        SelectedToolType = canvasToolType;
        ResetSelection();
    }

    public void AddShape(IShape? shape)  {
        Console.WriteLine("Adding shape " + shape);
        if(shape is not null)
            Canvas.Shapes.Add(shape);
    }
    public void SelectShape(IShape? shape) => SelectedShape = shape;
    public void RemoveShape(IShape? shape)
    {
        Console.WriteLine("Removing shape " + shape);
        if (shape is not null)
            Canvas.Shapes.Remove(shape);
        ResetSelection();
    }
    private void ResetSelection()
    {
        SelectedShape = null;
    }
}