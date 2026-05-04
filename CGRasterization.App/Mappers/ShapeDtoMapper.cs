using System;
using System.Drawing;
using System.Linq;
using CGRasterization.App.Dto;
using CGRasterization.App.Dto.Shapes;
using CGRasterization.App.Dto.Shapes.Abstractions;
using CGRasterization.Core.Primitives;
using CGRasterization.Core.Primitives.Abstractions;

namespace CGRasterization.App.Mappers;

public static class ShapeDtoMapper
{
    public static ShapeDto ToDto(this IShape shape)
    {
        return shape switch
        {
            Line line => line.ToDto(),
            Circle circle => circle.ToDto(),
            Polygon polygon => polygon.ToDto(),
            _ => throw new NotSupportedException($"Cannot convert shape of type {shape.GetType().Name} to DTO.")
        };
    }
    public static IShape FromDto(this ShapeDto dto)
    {
        return dto switch
        {
            LineDto line => line.FromDto(),
            CircleDto circle => circle.FromDto(),
            PolygonDto polygon => polygon.FromDto(),
            _ => throw new NotSupportedException($"Cannot convert DTO of type {dto.GetType().Name} to shape.")
        };
    }
    private static LineDto ToDto(this Line line)
    {
        return new LineDto(
            Start: line.Start.ToDto(),
            End: line.End.ToDto(),
            Thickness: line.Thickness,
            Color: line.Color.ToDto()
        );
    }
    private static CircleDto ToDto(this Circle circle)
    {
        return new CircleDto(
            Center: circle.Center.ToDto(),
            Radius: circle.Radius,
            Thickness: circle.Thickness,
            Color: circle.Color.ToDto()
        );
    }
    private static PolygonDto ToDto(this Polygon polygon)
    {
        return new PolygonDto(
            Points: polygon.Vertices.Select(p => p.ToDto()).ToList(),
            Thickness: polygon.Thickness,
            Color: polygon.Color.ToDto(),
            IsClosed: polygon.IsClosed
        );
    }
    private static Line FromDto(this LineDto line)
    {
        return new Line(
            line.Start.ToPoint(),
            line.End.ToPoint(),
            line.Color.ToColor(),
            line.Thickness
        );
    }
    private static Circle FromDto(this CircleDto circle)
    {
        return new Circle(
            circle.Center.ToPoint(),
            radius: circle.Radius,
            circle.Color.ToColor(),
            circle.Thickness
        );
    }
    private static Polygon FromDto(this PolygonDto polygon)
    {
        return new Polygon(
            polygon.Points.Select(p => p.ToPoint()).ToList(),
            polygon.IsClosed,
            polygon.Color.ToColor(),
            polygon.Thickness
        );
    }
    private static PointDto ToDto(this Point point) => new PointDto(point.X, point.Y);
    private static Point ToPoint(this PointDto dto) => new Point(dto.X, dto.Y);
    private static ColorDto ToDto(this Color color)
    {
        return new ColorDto(
            R: color.R,
            G: color.G,
            B: color.B,
            A: color.A
        );
    }
    private static Color ToColor(this ColorDto dto) => Color.FromArgb(dto.A, dto.R, dto.G, dto.B);
}