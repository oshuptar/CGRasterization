using System.Collections.Generic;
using CGRasterization.App.Dto.Shapes.Abstractions;

namespace CGRasterization.App.Dto.Shapes;

public record PolygonDto(List<PointDto> Points, int Thickness, ColorDto Color, bool IsClosed, ColorDto? FillColor = null) : ShapeDto(Thickness, Color);