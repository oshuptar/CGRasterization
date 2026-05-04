using System.Threading.Tasks;
using CGRasterization.App.Dto;

namespace CGRasterization.App.Services.Asbtractions;

public interface ICanvasPersistenceService
{
    Task SaveAsync(Canvas.Canvas canvas, string filePath);
    Task<CanvasDto?> LoadAsync(string filePath);
}