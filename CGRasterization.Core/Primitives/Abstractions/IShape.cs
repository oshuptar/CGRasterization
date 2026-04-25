using CGRasterization.Core.Abstractions;

namespace CGRasterization.Core.Primitives.Abstractions;

public interface IShape : ISelectable, IDrawable
{
    public int Thickness { get; set; }
}