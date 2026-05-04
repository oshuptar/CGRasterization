using System.Collections.Generic;
using CGRasterization.App.Dto.Shapes.Abstractions;

namespace CGRasterization.App.Dto;

public record  CanvasDto(int Width, int Height, IEnumerable<ShapeDto> Shapes);
