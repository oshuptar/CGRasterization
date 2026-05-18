using System.Text.Json.Serialization;

namespace CGRasterization.App.Dto.Shapes.Abstractions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "shapeType")]
[JsonDerivedType(typeof(LineDto), "line")]
[JsonDerivedType(typeof(CircleDto), "circle")]
[JsonDerivedType(typeof(PolygonDto), "polygon")]
[JsonDerivedType(typeof(RectangleDto), "rectangle")]
public abstract record ShapeDto(int Thickness, ColorDto Color);