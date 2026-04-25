using System.Drawing;

namespace CGRasterization.Core.Abstractions;

public interface ISelectable
{
    public double DistanceTo(Point point);
}