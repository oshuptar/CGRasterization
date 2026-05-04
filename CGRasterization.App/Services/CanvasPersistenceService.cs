using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CGRasterization.App.Dto;
using CGRasterization.App.Mappers;
using CGRasterization.App.Services.Asbtractions;

namespace CGRasterization.App.Services;

public sealed class CanvasPersistenceService : ICanvasPersistenceService
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    public async Task SaveAsync(Canvas.Canvas canvas, string filePath)
    {
        CanvasDto dto = canvas.ToDto();
        string json = JsonSerializer.Serialize(dto, _options);
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<CanvasDto> LoadAsync(string filePath)
    {
        string json = await File.ReadAllTextAsync(filePath);
        CanvasDto? dto = JsonSerializer.Deserialize<CanvasDto>(json, _options);
        if (dto is null) throw new InvalidOperationException("Could not deserialize canvas file.");
        return dto;
    }
}