using CGRasterization.Core.Primitives;
using CGRasterization.Core.Rasterizers.Abstractions;

namespace CGRasterization.Core.Rasterizers.Factory;

public class RasterizerFactory
{
    public IRasterizer<Line> GetLineRasterizer() => new LineRasterizer();
}