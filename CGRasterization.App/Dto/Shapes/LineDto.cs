using CGRasterization.App.Dto.Shapes.Abstractions;

namespace CGRasterization.App.Dto.Shapes;

public record LineDto(PointDto Start, PointDto End, int Thickness, ColorDto Color): ShapeDto(Thickness, Color);