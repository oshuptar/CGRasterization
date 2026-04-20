using CGRasterization.Core.Primitives;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Rasterizers.Factory;

public class RasterizerFactory
{
    public IRasterizer<TShape>? GetRasterizer<TShape>()
    {
        if (typeof(TShape) == typeof(Line))
            return new LineRasterizer() as IRasterizer<TShape>;
        if (typeof(TShape) == typeof(Circle))
            return new CircleRasterizer() as IRasterizer<TShape>;
        return null;
    }
}