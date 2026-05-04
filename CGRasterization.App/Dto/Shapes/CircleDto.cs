using CGRasterization.App.Dto.Shapes.Abstractions;

namespace CGRasterization.App.Dto.Shapes;

public record CircleDto(PointDto Center, int Radius, int Thickness, ColorDto Color):ShapeDto(Thickness, Color);