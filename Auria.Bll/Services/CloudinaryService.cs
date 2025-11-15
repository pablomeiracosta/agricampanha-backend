using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Auria.Bll.Services.Interfaces;
using Auria.Structure;
using Serilog;

namespace Auria.Bll.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger _logger;

    public CloudinaryService(AuriaContext context)
    {
        _logger = context.Log;

        var account = new Account(
            context.Settings.Cloudinary.CloudName,
            context.Settings.Cloudinary.ApiKey,
            context.Settings.Cloudinary.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile file, string folder = "noticias")
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Arquivo inválido");
            }

            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.Information("Imagem enviada com sucesso para Cloudinary: {Url}", uploadResult.SecureUrl);
                return uploadResult.SecureUrl.ToString();
            }

            _logger.Error("Erro ao fazer upload da imagem: {Error}", uploadResult.Error?.Message);
            throw new Exception($"Erro ao fazer upload da imagem: {uploadResult.Error?.Message}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao fazer upload da imagem no Cloudinary");
            throw;
        }
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return false;
            }

            // Extrai o publicId da URL
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');
            var publicIdWithExtension = string.Join("/", segments.Skip(segments.Length - 2));
            var publicId = publicIdWithExtension.Substring(0, publicIdWithExtension.LastIndexOf('.'));

            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.Result == "ok")
            {
                _logger.Information("Imagem deletada com sucesso do Cloudinary: {PublicId}", publicId);
                return true;
            }

            _logger.Warning("Não foi possível deletar a imagem do Cloudinary: {PublicId}", publicId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao deletar imagem do Cloudinary: {ImageUrl}", imageUrl);
            return false;
        }
    }
}
