using System.IO;
using CGRasterization.Core.ImagePattern;

namespace CGRasterization.App.Services.Asbtractions;

public interface IFillImageService
{
    ImagePattern? Load(Stream stream);
}
