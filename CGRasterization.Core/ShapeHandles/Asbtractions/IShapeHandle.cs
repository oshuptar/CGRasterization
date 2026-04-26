using System.Drawing;

namespace CGRasterization.Core.ShapeHandles.Asbtractions;

public interface IShapeHandle
{
    Point Position { get; }
    void MoveBy(int dx, int dy);
}