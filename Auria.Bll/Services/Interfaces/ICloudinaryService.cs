using Microsoft.AspNetCore.Http;

namespace Auria.Bll.Services.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file, string folder = "noticias");
    Task<bool> DeleteImageAsync(string imageUrl);
}
