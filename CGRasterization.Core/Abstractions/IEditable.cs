using System.Drawing;
using CGRasterization.Core.ShapeHandles.Asbtractions;

namespace CGRasterization.Core.Abstractions;

public interface IEditable
{
    IShapeHandle? GetHandle(Point point, double tolerance);
    void MoveHandle(IShapeHandle handle, int dx, int dy);
}