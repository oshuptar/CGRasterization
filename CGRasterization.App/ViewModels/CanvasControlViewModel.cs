using System.Drawing;
using CGRasterization.App.Canvas.Enums;
using CGRasterization.App.ViewModels.Abstractions;
using CGRasterization.Core.Primitives;
using CommunityToolkit.Mvvm.Input;

namespace CGRasterization.App.ViewModels;

public class CanvasControlViewModel : ViewModelBase
{
    public Canvas.Canvas Canvas { get; set; } = new(1200, 800);

    public ShapeType ShapeType
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsCircleShape));
            OnPropertyChanged(nameof(IsLineShape));
            OnPropertyChanged(nameof(IsMoveShape));
        }
    }

    public bool IsLineShape => ShapeType == ShapeType.Line;
    public bool IsCircleShape => ShapeType == ShapeType.Circle;
    public bool IsMoveShape => ShapeType == ShapeType.None;
    public RelayCommand SetLineShapeCommand { get; }
    public RelayCommand SetCircleShapeCommand { get; }
    public RelayCommand SetMoveCommand { get; }

    public CanvasControlViewModel()
    {
        SetLineShapeCommand = new RelayCommand(() => SetShapeType(ShapeType.Line), () => true);
        SetCircleShapeCommand = new RelayCommand(() => SetShapeType(ShapeType.Circle), () => true);
        SetMoveCommand = new RelayCommand(() => SetShapeType(ShapeType.None), () => true);
        ShapeType = ShapeType.None;
    }

    public void SetShapeType(ShapeType shapeType)
    {
        ShapeType = shapeType;
    }
    public void AddShape(Point startPoint, Point endPoint)
    {
        if (ShapeType == ShapeType.Line)
        {
            Line newLine = new Line(startPoint, endPoint);
            Canvas.Lines.Add(newLine);
        }
        else if (ShapeType == ShapeType.Circle)
        {
            Circle newCircle = new Circle(startPoint, endPoint);
            Canvas.Circles.Add(newCircle);
        }
    }
}