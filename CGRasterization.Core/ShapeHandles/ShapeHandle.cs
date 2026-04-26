using System.Drawing;
using CGRasterization.Core.ShapeHandles.Asbtractions;

namespace CGRasterization.Core.ShapeHandles;

public sealed class ShapeHandle : IShapeHandle
{
    private readonly Func<Point> _getPosition;
    private readonly Action<Point> _setPosition;
    public Point Position
    {
        get => _getPosition();
        set => _setPosition(value);
    }
    public ShapeHandle(Func<Point> getPosition, Action<Point> setPosition)
    {
        _getPosition = getPosition;
        _setPosition = setPosition;
    }
    public void MoveBy(int dx, int dy)
    {
        Point current = Position;
        Position = new Point(current.X + dx, current.Y + dy);
    }
}