using CGRasterization.App.Dto.Shapes.Abstractions;

namespace CGRasterization.App.Dto.Shapes;

public record RectangleDto(PointDto TopLeft, PointDto BottomRight, int Thickness, ColorDto Color)
    : ShapeDto(Thickness, Color);
