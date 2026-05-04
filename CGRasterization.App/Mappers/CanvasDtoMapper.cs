using System.Collections.Generic;
using System.Linq;
using CGRasterization.App.Dto;
using CGRasterization.Core.Primitives.Abstractions;

namespace CGRasterization.App.Mappers;

public static class CanvasDtoMapper
{
    public static CanvasDto ToDto(this Canvas.Canvas canvas)
    {
        return new CanvasDto(
            Width: canvas.Width,
            Height: canvas.Height,
            Shapes: canvas.Shapes
                .Select(shape => shape.ToDto())
                .ToList()
        );
    }
    public static List<IShape> FromDto(this CanvasDto dto)
    {
        return dto.Shapes
            .Select(shapeDto => shapeDto.FromDto())
            .ToList();
    }
}