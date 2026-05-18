using CGRasterization.Core.Primitives;
using CGRasterization.Core.Primitives.Abstractions;

namespace CGRasterization.App.Clipping;

public record ClipOperation(Polygon ClippingPolygon, IShape ClipWindow);
