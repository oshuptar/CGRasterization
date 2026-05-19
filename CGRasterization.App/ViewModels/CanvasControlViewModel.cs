using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CGRasterization.App.Canvas.Enums;
using CGRasterization.App.Canvas.Tools;
using CGRasterization.App.Canvas.Tools.Abstractions;
using CGRasterization.App.Clipping;
using CGRasterization.App.Dto;
using CGRasterization.App.Mappers;
using CGRasterization.App.Services;
using CGRasterization.App.Services.Asbtractions;
using CGRasterization.App.ViewModels.Abstractions;
using CGRasterization.Core.Clipping;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Primitives.Abstractions;
using CommunityToolkit.Mvvm.Input;
using Color = System.Drawing.Color;

namespace CGRasterization.App.ViewModels;

public class CanvasControlViewModel : ViewModelBase
{
    private readonly ICanvasPersistenceService _canvasPersistenceService = new CanvasPersistenceService();
    private readonly IFillImageService _fillImageService = new FillImageService();
    public Canvas.Canvas Canvas { get; set; } = new(1200, 800);
    public Avalonia.Media.Color SelectedShapeBrushColorPicker
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            if (SelectedShape != null)
            {
                SelectedShape.Color = Color.FromArgb(value.A, value.R, value.G, value.B);
                Canvas.RedrawShapes();
            }
            OnPropertyChanged();
        }
    }
    public int SelectedShapeThickness
    {
        get;
        set
        {
            int val = Math.Max(1, value);
            if (field == val) return;
            field = val;
            if (SelectedShape != null)
            {
                SelectedShape.Thickness = val;
                Canvas.RedrawShapes();
            }
            OnPropertyChanged();
        }
    }
    public bool IsSelectedPolygon => SelectedShape is Polygon; // Pattern - matching if a selected shape is a polygon
    public bool HasFillImage => (SelectedShape as Polygon)?.FillImage != null;
    public bool SelectedShapeFillEnabled
    {
        get => (SelectedShape as Polygon)?.FillColor != null || (SelectedShape as Polygon)?.FillImage != null;
        set
        {
            if (SelectedShape is not Polygon polygon) return;
            polygon.FillColor = value
                ? Color.FromArgb(SelectedShapeFillColorPicker.A, SelectedShapeFillColorPicker.R, SelectedShapeFillColorPicker.G, SelectedShapeFillColorPicker.B)
                : null;
            if (!value) polygon.FillImage = null;
            Canvas.RedrawShapes();
            OnPropertyChanged();
        }
    }
    public Avalonia.Media.Color SelectedShapeFillColorPicker
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            if (SelectedShape is Polygon polygon)
            {
                polygon.FillColor = Color.FromArgb(value.A, value.R, value.G, value.B);
                Canvas.RedrawShapes();
            }
            OnPropertyChanged();
        }
    } = Avalonia.Media.Colors.White;
    
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
                if (field is Polygon p && p.FillColor != null)
                    SelectedShapeFillColorPicker = Avalonia.Media.Color.FromArgb(p.FillColor.Value.A, p.FillColor.Value.R, p.FillColor.Value.G, p.FillColor.Value.B);
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
    public bool IsDrawPolygonMode => SelectedToolType == CanvasToolType.DrawPolygon;
    public bool IsDrawRectangleMode => SelectedToolType == CanvasToolType.DrawRectangle;
    public bool IsMoveShapeMode => SelectedToolType == CanvasToolType.MoveShape;
    public bool IsEditShapeMode => SelectedToolType == CanvasToolType.EditShape;
    public IShape? ClipWindowShape
    {
        get => field;
        private set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasClipWindow));
        }
    }
    public bool HasClipWindow => ClipWindowShape is not null;
    public RelayCommand SetLineDrawToolCommand { get; }
    public RelayCommand SetCircleDrawToolCommand { get; }
    public RelayCommand SetPolygonDrawToolCommand { get; }
    public RelayCommand SetRectangleDrawToolCommand { get; }
    public RelayCommand SetSelectShapeToolCommand { get; }
    public RelayCommand RemoveShapeCommand { get; }
    public RelayCommand MoveShapeCommand { get; }
    public RelayCommand EditShapeCommand { get; }
    public RelayCommand SetClipWindowCommand { get; }
    public RelayCommand ApplyCyrusBeckCommand { get; }
    public RelayCommand ClearClipCommand { get; }
    public RelayCommand ClearFillImageCommand { get; }
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
        SetPolygonDrawToolCommand = new RelayCommand(() =>
        {
            SetToolMode(CanvasToolType.DrawPolygon);
            ResetSelection();
        }, () => true);
        SetRectangleDrawToolCommand = new RelayCommand(() =>
        {
            SetToolMode(CanvasToolType.DrawRectangle);
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
        SetClipWindowCommand = new RelayCommand(() =>
        {
            if (SelectedShape is not Polygon and not Rectangle) return;
            ClipWindowShape = SelectedShape;
        }, () => true);
        ApplyCyrusBeckCommand = new RelayCommand(() =>
        {
            if (SelectedShape is not Polygon clippingPolygon) return;
            if (ClipWindowShape is null || ClipWindowShape == SelectedShape) return;
            var clipPoints = CyrusBeckClipper.GetClipWindowPoints(ClipWindowShape);
            if (clipPoints is null) return;
            if (!CyrusBeckClipper.IsConvex(clipPoints)) return;
            Canvas.ActiveClipOperation = new ClipOperation(clippingPolygon, ClipWindowShape);
        }, () => true);
        ClearClipCommand = new RelayCommand(() =>
        {
            Canvas.ActiveClipOperation = null;
            ClipWindowShape = null;
        }, () => true);
        ClearFillImageCommand = new RelayCommand(() =>
        {
            if (SelectedShape is not Polygon polygon) return;
            if (HasFillImage)
            {
                polygon.FillImage = null;
                Canvas.RedrawShapes();
                OnPropertyChanged(nameof(HasFillImage));
                OnPropertyChanged(nameof(SelectedShapeFillEnabled));
            }
        }, () => true);
        _tools = new Dictionary<CanvasToolType, ICanvasTool>
        {
            [CanvasToolType.DrawLine] = new DrawLineTool(),
            [CanvasToolType.DrawCircle] = new DrawCircleTool(),
            [CanvasToolType.SelectShape] = new SelectShapeTool(),
            [CanvasToolType.MoveShape] = new MoveShapeTool(),
            [CanvasToolType.EditShape] = new EditShapeTool(),
            [CanvasToolType.DrawPolygon] = new DrawPolygonTool(),
            [CanvasToolType.DrawRectangle] = new DrawRectangleTool(),
        };
        SelectedToolType = CanvasToolType.None;
    }
    public async Task SaveCanvasAsync() => await _canvasPersistenceService.SaveAsync(Canvas, "canvas.json");
    public async Task LoadCanvasAsync()
    {
        CanvasDto? dto = await _canvasPersistenceService.LoadAsync("canvas.json");
        if (dto is null) return;
        List<IShape> shapes = dto.FromDto();
        Canvas.ReplaceShapes(shapes);
        ResetSelection();
    }
    private void SetToolMode(CanvasToolType canvasToolType){ 
        CurrentTool?.Cancel();
        Canvas.SetPreviewShape(null);
        SelectedToolType = canvasToolType;
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
        if (shape is null) return;
        if (shape == ClipWindowShape)
        {
            Canvas.ActiveClipOperation = null;
            ClipWindowShape = null;
        }
        Canvas.Shapes.Remove(shape);
        ResetSelection();
    }
    private void ResetSelection() => SelectedShape = null;
    private void RefreshFlags()
    {
        OnPropertyChanged(nameof(IsDrawCircleMode));
        OnPropertyChanged(nameof(IsDrawLineMode));
        OnPropertyChanged(nameof(IsDrawPolygonMode));
        OnPropertyChanged(nameof(IsDrawRectangleMode));
        OnPropertyChanged(nameof(IsSelectShapeMode));
        OnPropertyChanged(nameof(IsMoveShapeMode));
        OnPropertyChanged(nameof(IsEditShapeMode));
    }
    public void RefreshSelected()
    {
        OnPropertyChanged(nameof(SelectedShape));
        OnPropertyChanged(nameof(IsSelectedShape));
        OnPropertyChanged(nameof(SelectedShapeControlsOpacity));
        OnPropertyChanged(nameof(IsSelectedPolygon));
        OnPropertyChanged(nameof(SelectedShapeFillEnabled));
        OnPropertyChanged(nameof(SelectedShapeFillColorPicker));
        OnPropertyChanged(nameof(HasFillImage));
    }

    public void SetFillImage(Stream stream)
    {
        if (SelectedShape is not Polygon polygon) return;
        var pattern = _fillImageService.Load(stream);
        if (pattern is null) return;
        polygon.FillImage = pattern;
        Canvas.RedrawShapes();
        OnPropertyChanged(nameof(SelectedShapeFillEnabled));
        OnPropertyChanged(nameof(HasFillImage));
    }

    public void ClearCanvas()
    {
        Canvas.ClearCanvas();
        ResetSelection();
    }
}