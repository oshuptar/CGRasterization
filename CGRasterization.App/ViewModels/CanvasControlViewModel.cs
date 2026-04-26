using System;
using System.Collections.Generic;
using CGRasterization.App.Canvas.Enums;
using CGRasterization.App.Canvas.Tools;
using CGRasterization.App.Canvas.Tools.Abstractions;
using CGRasterization.App.ViewModels.Abstractions;
using CGRasterization.Core.Primitives.Abstractions;
using CommunityToolkit.Mvvm.Input;
using Color = System.Drawing.Color;

namespace CGRasterization.App.ViewModels;

public class CanvasControlViewModel : ViewModelBase
{
    public Canvas.Canvas Canvas { get; set; } = new(1200, 800);
    public Avalonia.Media.Color SelectedShapeBrushColorPicker
    {
        get;
        set
        {
            field = value;
            if (SelectedShape != null)
            {
                SelectedShape.Color = Color.FromArgb(value.A, value.R, value.G, value.B);
                Canvas.RedrawShapes(); // TODO: modify to redraw only specific shape
            }
            OnPropertyChanged();
        }
    }
    public int SelectedShapeThickness
    {
        get;
        set
        {
            int normalized = value % 2 == 0 ? value + 1 : value;
            if (field == normalized) return;
            field = normalized;
            if (SelectedShape != null)
            {
                SelectedShape.Thickness = field;
                Canvas.RedrawShapes(); // TODO: modify to redraw only specific shape
            }
            OnPropertyChanged();
        }
    }
    public IShape? SelectedShape
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            if (field != null)
            {
                SelectedShapeBrushColorPicker = Avalonia.Media.Color.FromArgb(field.Color.A,field.Color.R,field.Color.G,field.Color.B);
                SelectedShapeThickness = field.Thickness;
            }
            RefreshSelected();
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
            RefreshFlags();
        }
    }
    public double SelectedShapeControlsOpacity => IsSelectedShape ? 1.0 : 0.0;
    public bool IsSelectedShape => SelectedShape != null;
    public bool IsSelectShapeMode => SelectedToolType == CanvasToolType.SelectShape;
    public bool IsDrawLineMode => SelectedToolType == CanvasToolType.DrawLine;
    public bool IsDrawCircleMode => SelectedToolType == CanvasToolType.DrawCircle;
    public bool IsMoveShapeMode => SelectedToolType == CanvasToolType.MoveShape;
    public bool IsEditShapeMode => SelectedToolType == CanvasToolType.EditShape;
    public RelayCommand SetLineDrawToolCommand { get; }
    public RelayCommand SetCircleDrawToolCommand { get; }
    public RelayCommand SetSelectShapeToolCommand { get; }
    public RelayCommand RemoveShapeCommand { get; }
    public RelayCommand MoveShapeCommand { get; }
    public RelayCommand EditShapeCommand { get; }
    private readonly Dictionary<CanvasToolType, ICanvasTool> _tools;
    public ICanvasTool? CurrentTool
    {
        get
        {
            if(SelectedToolType != CanvasToolType.None)
                return _tools[SelectedToolType];
            return null;
        }
    }
    public CanvasControlViewModel()
    {
        SetLineDrawToolCommand = new RelayCommand(() =>
        {
            SetToolMode(CanvasToolType.DrawLine);
            ResetSelection();
        }, () => true);
        SetCircleDrawToolCommand = new RelayCommand(() =>
        {
            SetToolMode(CanvasToolType.DrawCircle);
            ResetSelection();
        }, () => true);
        SetSelectShapeToolCommand = new RelayCommand(() =>
        {
            SetToolMode(CanvasToolType.SelectShape);
            ResetSelection();
        }, () => true);
        RemoveShapeCommand = new RelayCommand(() =>
        {
            RemoveShape(SelectedShape);
            ResetSelection();
        }, () => true);
        MoveShapeCommand = new RelayCommand(() => SetToolMode(CanvasToolType.MoveShape), () => true);
        EditShapeCommand = new RelayCommand(() => SetToolMode(CanvasToolType.EditShape), () => true);
        _tools = new Dictionary<CanvasToolType, ICanvasTool>
        {
            [CanvasToolType.DrawLine] = new DrawLineTool(),
            [CanvasToolType.DrawCircle] = new DrawCircleTool(),
            [CanvasToolType.SelectShape] = new SelectShapeTool(),
            [CanvasToolType.MoveShape] = new MoveShapeTool(),
            [CanvasToolType.EditShape] = new EditShapeTool()
        };
        SelectedToolType = CanvasToolType.None;
    }
    public void SetToolMode(CanvasToolType canvasToolType) => SelectedToolType = canvasToolType;
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
        ResetToolMode();
    }
    private void ResetSelection() => SelectedShape = null;

    private void RefreshFlags()
    {
        OnPropertyChanged(nameof(IsDrawCircleMode));
        OnPropertyChanged(nameof(IsDrawLineMode));
        OnPropertyChanged(nameof(IsSelectShapeMode));
        OnPropertyChanged(nameof(IsMoveShapeMode));
        OnPropertyChanged(nameof(IsEditShapeMode));
    }
    public void RefreshSelected()
    {
        OnPropertyChanged(nameof(SelectedShape));
        OnPropertyChanged(nameof(IsSelectedShape));
        OnPropertyChanged(nameof(SelectedShapeControlsOpacity));
    }

    public void ResetToolMode()
    {
        SetToolMode(CanvasToolType.None);
        ResetSelection();
    }
}