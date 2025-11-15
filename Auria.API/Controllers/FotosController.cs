using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auria.Bll.Services.Interfaces;
using Auria.Structure;

namespace Auria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FotosController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;
    private readonly AuriaContext _context;

    public FotosController(ICloudinaryService cloudinaryService, AuriaContext context)
    {
        _cloudinaryService = cloudinaryService;
        _context = context;
    }

    /// <summary>
    /// Faz upload de uma foto para o Cloudinary
    /// </summary>
    /// <param name="file">Arquivo de imagem</param>
    /// <param name="folder">Pasta de destino no Cloudinary (padrão: noticias)</param>
    /// <returns>URL da foto enviada</returns>
    [HttpPost("upload")]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100 MB
    [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
    [ProducesResponseType(typeof(FotoUploadResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<FotoUploadResponseDto>> Upload(
        [FromForm] IFormFile file,
        [FromForm] string folder = "noticias")
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Nenhum arquivo fornecido" });
            }

            // Validar tipo de arquivo
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { message = "Tipo de arquivo não permitido. Use: jpg, jpeg, png, gif ou webp" });
            }

            // Validar tamanho (máximo 50MB)
            if (file.Length > 50 * 1024 * 1024)
            {
                return BadRequest(new { message = "Arquivo muito grande. Tamanho máximo: 50MB" });
            }

            _context.Log.Information("Upload de foto iniciado: {FileName}, Tamanho: {Size} bytes", file.FileName, file.Length);

            var imageUrl = await _cloudinaryService.UploadImageAsync(file, folder);

            _context.Log.Information("Upload de foto concluído: {Url}", imageUrl);

            return Ok(new FotoUploadResponseDto
            {
                Url = imageUrl,
                FileName = file.FileName,
                Size = file.Length,
                UploadedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao fazer upload da foto");
            return StatusCode(500, new { message = "Erro ao fazer upload da foto", error = ex.Message });
        }
    }

    /// <summary>
    /// Deleta uma foto do Cloudinary
    /// </summary>
    /// <param name="imageUrl">URL da imagem a ser deletada</param>
    /// <returns>Status da operação</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromQuery] string imageUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest(new { message = "URL da imagem não fornecida" });
            }

            _context.Log.Information("Solicitação de deleção de foto: {Url}", imageUrl);

            var result = await _cloudinaryService.DeleteImageAsync(imageUrl);

            if (result)
            {
                _context.Log.Information("Foto deletada com sucesso: {Url}", imageUrl);
                return Ok(new { message = "Foto deletada com sucesso" });
            }

            _context.Log.Warning("Não foi possível deletar a foto: {Url}", imageUrl);
            return BadRequest(new { message = "Não foi possível deletar a foto" });
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao deletar foto: {Url}", imageUrl);
            return StatusCode(500, new { message = "Erro ao deletar foto", error = ex.Message });
        }
    }

    /// <summary>
    /// Faz upload de múltiplas fotos
    /// </summary>
    /// <param name="files">Lista de arquivos de imagem</param>
    /// <param name="folder">Pasta de destino no Cloudinary (padrão: noticias)</param>
    /// <returns>Lista de URLs das fotos enviadas</returns>
    [HttpPost("upload/multiplas")]
    [ProducesResponseType(typeof(List<FotoUploadResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<List<FotoUploadResponseDto>>> UploadMultiplas(
        [FromForm] List<IFormFile> files,
        [FromForm] string folder = "noticias")
    {
        try
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest(new { message = "Nenhum arquivo fornecido" });
            }

            if (files.Count > 10)
            {
                return BadRequest(new { message = "Máximo de 10 arquivos por upload" });
            }

            var results = new List<FotoUploadResponseDto>();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            foreach (var file in files)
            {
                if (file == null || file.Length == 0) continue;

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    _context.Log.Warning("Arquivo ignorado (tipo inválido): {FileName}", file.FileName);
                    continue;
                }

                if (file.Length > 10 * 1024 * 1024)
                {
                    _context.Log.Warning("Arquivo ignorado (muito grande): {FileName}", file.FileName);
                    continue;
                }

                try
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(file, folder);
                    results.Add(new FotoUploadResponseDto
                    {
                        Url = imageUrl,
                        FileName = file.FileName,
                        Size = file.Length,
                        UploadedAt = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    _context.Log.Error(ex, "Erro ao fazer upload do arquivo: {FileName}", file.FileName);
                }
            }

            _context.Log.Information("Upload múltiplo concluído: {Count} fotos enviadas", results.Count);

            return Ok(results);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao fazer upload múltiplo de fotos");
            return StatusCode(500, new { message = "Erro ao fazer upload múltiplo", error = ex.Message });
        }
    }
}

public class FotoUploadResponseDto
{
    public string Url { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
}
