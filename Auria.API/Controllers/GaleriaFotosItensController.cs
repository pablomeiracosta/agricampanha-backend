using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Auria.Data.Context;
using Auria.Data.Entities;
using Auria.Dto.GaleriaFotos;
using Auria.Structure;
using Auria.Bll.Services.Interfaces;

namespace Auria.API.Controllers;

[ApiController]
[Route("api/galeria-fotos/{idGaleria}/fotos")]
[Authorize]
public class GaleriaFotosItensController : ControllerBase
{
    private readonly AuriaDbContext _context;
    private readonly IMapper _mapper;
    private readonly AuriaContext _auriaContext;
    private readonly ICloudinaryService _cloudinaryService;

    public GaleriaFotosItensController(
        AuriaDbContext context,
        IMapper mapper,
        AuriaContext auriaContext,
        ICloudinaryService cloudinaryService)
    {
        _context = context;
        _mapper = mapper;
        _auriaContext = auriaContext;
        _cloudinaryService = cloudinaryService;
    }

    /// <summary>
    /// Retorna todas as fotos de uma galeria
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <returns>Lista de fotos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<FotoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<FotoDto>>> GetFotosPorGaleria(int idGaleria)
    {
        try
        {
            _auriaContext.Log.Information("Buscando fotos da galeria ID: {IdGaleria}", idGaleria);

            var galeria = await _context.GaleriasFotos.FindAsync(idGaleria);
            if (galeria == null)
            {
                return NotFound(new { message = "Galeria não encontrada" });
            }

            var fotos = await _context.Fotos
                .Where(f => f.IdGaleriaFotos == idGaleria)
                .OrderBy(f => f.Ordem)
                .ThenByDescending(f => f.DataUpload)
                .ToListAsync();

            var fotosDto = _mapper.Map<List<FotoDto>>(fotos);
            return Ok(fotosDto);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao buscar fotos da galeria ID: {IdGaleria}", idGaleria);
            return StatusCode(500, new { message = "Erro ao buscar fotos", error = ex.Message });
        }
    }

    /// <summary>
    /// Retorna uma foto específica
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <param name="idFoto">ID da foto</param>
    /// <returns>Foto encontrada</returns>
    [HttpGet("{idFoto}")]
    [ProducesResponseType(typeof(FotoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FotoDto>> GetFotoById(int idGaleria, int idFoto)
    {
        try
        {
            _auriaContext.Log.Information("Buscando foto ID: {IdFoto} da galeria: {IdGaleria}", idFoto, idGaleria);

            var foto = await _context.Fotos
                .FirstOrDefaultAsync(f => f.Id == idFoto && f.IdGaleriaFotos == idGaleria);

            if (foto == null)
            {
                return NotFound(new { message = "Foto não encontrada" });
            }

            var fotoDto = _mapper.Map<FotoDto>(foto);
            return Ok(fotoDto);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao buscar foto ID: {IdFoto}", idFoto);
            return StatusCode(500, new { message = "Erro ao buscar foto", error = ex.Message });
        }
    }

    /// <summary>
    /// Adiciona uma foto à galeria (faz upload e registra no banco)
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <param name="file">Arquivo de imagem</param>
    /// <param name="legenda">Legenda da foto (opcional)</param>
    /// <param name="ordem">Ordem da foto na galeria (opcional)</param>
    /// <returns>Foto adicionada</returns>
    [HttpPost]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100 MB
    [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
    [ProducesResponseType(typeof(FotoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<FotoDto>> AdicionarFoto(
        int idGaleria,
        [FromForm] IFormFile file,
        [FromForm] string? legenda = null,
        [FromForm] int? ordem = null)
    {
        try
        {
            // Verificar se a galeria existe
            var galeria = await _context.GaleriasFotos.FindAsync(idGaleria);
            if (galeria == null)
            {
                return NotFound(new { message = "Galeria não encontrada" });
            }

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

            _auriaContext.Log.Information("Upload de foto iniciado para galeria {IdGaleria}: {FileName}, Tamanho: {Size} bytes",
                idGaleria, file.FileName, file.Length);

            // Upload para Cloudinary
            var imageUrl = await _cloudinaryService.UploadImageAsync(file, "galerias");

            // Criar registro da foto
            var foto = new Foto
            {
                IdGaleriaFotos = idGaleria,
                Url = imageUrl,
                NomeArquivo = file.FileName,
                Legenda = legenda,
                Tamanho = file.Length,
                Ordem = ordem,
                DataUpload = DateTime.Now
            };

            _context.Fotos.Add(foto);
            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Foto adicionada à galeria {IdGaleria}: {Url}", idGaleria, imageUrl);

            var fotoDto = _mapper.Map<FotoDto>(foto);
            return CreatedAtAction(nameof(GetFotoById), new { idGaleria, idFoto = foto.Id }, fotoDto);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao adicionar foto à galeria {IdGaleria}", idGaleria);
            return StatusCode(500, new { message = "Erro ao adicionar foto", error = ex.Message });
        }
    }

    /// <summary>
    /// Adiciona uma foto existente do Cloudinary à galeria
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <param name="fotoDto">Dados da foto</param>
    /// <returns>Foto adicionada</returns>
    [HttpPost("adicionar-existente")]
    [ProducesResponseType(typeof(FotoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FotoDto>> AdicionarFotoExistente(
        int idGaleria,
        [FromBody] FotoCreateDto fotoDto)
    {
        try
        {
            // Verificar se a galeria existe
            var galeria = await _context.GaleriasFotos.FindAsync(idGaleria);
            if (galeria == null)
            {
                return NotFound(new { message = "Galeria não encontrada" });
            }

            _auriaContext.Log.Information("Adicionando foto existente à galeria {IdGaleria}: {Url}", idGaleria, fotoDto.Url);

            var foto = _mapper.Map<Foto>(fotoDto);
            foto.IdGaleriaFotos = idGaleria;
            foto.DataUpload = DateTime.Now;

            _context.Fotos.Add(foto);
            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Foto existente adicionada à galeria {IdGaleria}", idGaleria);

            var resultado = _mapper.Map<FotoDto>(foto);
            return CreatedAtAction(nameof(GetFotoById), new { idGaleria, idFoto = foto.Id }, resultado);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao adicionar foto existente à galeria {IdGaleria}", idGaleria);
            return StatusCode(500, new { message = "Erro ao adicionar foto", error = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza informações de uma foto (legenda, ordem)
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <param name="idFoto">ID da foto</param>
    /// <param name="fotoDto">Dados atualizados</param>
    /// <returns>Foto atualizada</returns>
    [HttpPut("{idFoto}")]
    [ProducesResponseType(typeof(FotoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FotoDto>> AtualizarFoto(
        int idGaleria,
        int idFoto,
        [FromBody] FotoUpdateDto fotoDto)
    {
        try
        {
            _auriaContext.Log.Information("Atualizando foto ID: {IdFoto} da galeria: {IdGaleria}", idFoto, idGaleria);

            var foto = await _context.Fotos
                .FirstOrDefaultAsync(f => f.Id == idFoto && f.IdGaleriaFotos == idGaleria);

            if (foto == null)
            {
                return NotFound(new { message = "Foto não encontrada" });
            }

            _mapper.Map(fotoDto, foto);
            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Foto atualizada com sucesso: {IdFoto}", idFoto);

            var resultado = _mapper.Map<FotoDto>(foto);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao atualizar foto ID: {IdFoto}", idFoto);
            return StatusCode(500, new { message = "Erro ao atualizar foto", error = ex.Message });
        }
    }

    /// <summary>
    /// Remove uma foto da galeria (e opcionalmente do Cloudinary)
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <param name="idFoto">ID da foto</param>
    /// <param name="deletarDoCloudinary">Se true, deleta também do Cloudinary (padrão: false)</param>
    /// <returns>Status da operação</returns>
    [HttpDelete("{idFoto}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverFoto(
        int idGaleria,
        int idFoto,
        [FromQuery] bool deletarDoCloudinary = false)
    {
        try
        {
            _auriaContext.Log.Information("Removendo foto ID: {IdFoto} da galeria: {IdGaleria}", idFoto, idGaleria);

            var foto = await _context.Fotos
                .FirstOrDefaultAsync(f => f.Id == idFoto && f.IdGaleriaFotos == idGaleria);

            if (foto == null)
            {
                return NotFound(new { message = "Foto não encontrada" });
            }

            // Deletar do Cloudinary se solicitado
            if (deletarDoCloudinary && !string.IsNullOrEmpty(foto.Url))
            {
                _auriaContext.Log.Information("Deletando foto do Cloudinary: {Url}", foto.Url);
                await _cloudinaryService.DeleteImageAsync(foto.Url);
            }

            _context.Fotos.Remove(foto);
            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Foto removida com sucesso: {IdFoto}", idFoto);
            return Ok(new { message = "Foto removida com sucesso" });
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao remover foto ID: {IdFoto}", idFoto);
            return StatusCode(500, new { message = "Erro ao remover foto", error = ex.Message });
        }
    }

    /// <summary>
    /// Reordena as fotos de uma galeria
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <param name="ordenacao">Lista com IDs das fotos na nova ordem</param>
    /// <returns>Status da operação</returns>
    [HttpPut("reordenar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReordenarFotos(
        int idGaleria,
        [FromBody] List<int> ordenacao)
    {
        try
        {
            _auriaContext.Log.Information("Reordenando fotos da galeria ID: {IdGaleria}", idGaleria);

            var galeria = await _context.GaleriasFotos
                .Include(g => g.Fotos)
                .FirstOrDefaultAsync(g => g.Id == idGaleria);

            if (galeria == null)
            {
                return NotFound(new { message = "Galeria não encontrada" });
            }

            // Atualizar ordem de cada foto
            for (int i = 0; i < ordenacao.Count; i++)
            {
                var foto = galeria.Fotos.FirstOrDefault(f => f.Id == ordenacao[i]);
                if (foto != null)
                {
                    foto.Ordem = i + 1;
                }
            }

            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Fotos reordenadas com sucesso na galeria: {IdGaleria}", idGaleria);
            return Ok(new { message = "Fotos reordenadas com sucesso" });
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao reordenar fotos da galeria ID: {IdGaleria}", idGaleria);
            return StatusCode(500, new { message = "Erro ao reordenar fotos", error = ex.Message });
        }
    }

    /// <summary>
    /// Define uma foto como principal da galeria (desmarca as outras)
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <param name="idFoto">ID da foto a ser definida como principal</param>
    /// <returns>Status da operação</returns>
    [HttpPut("{idFoto}/definir-principal")]
    [ProducesResponseType(typeof(FotoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FotoDto>> DefinirFotoPrincipal(int idGaleria, int idFoto)
    {
        try
        {
            _auriaContext.Log.Information("Definindo foto ID: {IdFoto} como principal da galeria: {IdGaleria}", idFoto, idGaleria);

            // Buscar galeria com suas fotos
            var galeria = await _context.GaleriasFotos
                .Include(g => g.Fotos)
                .FirstOrDefaultAsync(g => g.Id == idGaleria);

            if (galeria == null)
            {
                return NotFound(new { message = "Galeria não encontrada" });
            }

            // Buscar a foto específica
            var foto = galeria.Fotos.FirstOrDefault(f => f.Id == idFoto);
            if (foto == null)
            {
                return NotFound(new { message = "Foto não encontrada nesta galeria" });
            }

            // Desmarcar todas as fotos como principal
            foreach (var f in galeria.Fotos)
            {
                f.IsPrincipal = false;
            }

            // Marcar a foto especificada como principal
            foto.IsPrincipal = true;

            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Foto definida como principal: {IdFoto} na galeria: {IdGaleria}", idFoto, idGaleria);

            var fotoDto = _mapper.Map<FotoDto>(foto);
            return Ok(fotoDto);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao definir foto principal ID: {IdFoto}", idFoto);
            return StatusCode(500, new { message = "Erro ao definir foto principal", error = ex.Message });
        }
    }

    /// <summary>
    /// Retorna a foto principal da galeria
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <returns>Foto principal ou null se não houver</returns>
    [HttpGet("principal")]
    [ProducesResponseType(typeof(FotoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FotoDto>> GetFotoPrincipal(int idGaleria)
    {
        try
        {
            _auriaContext.Log.Information("Buscando foto principal da galeria ID: {IdGaleria}", idGaleria);

            var galeria = await _context.GaleriasFotos.FindAsync(idGaleria);
            if (galeria == null)
            {
                return NotFound(new { message = "Galeria não encontrada" });
            }

            var fotoPrincipal = await _context.Fotos
                .FirstOrDefaultAsync(f => f.IdGaleriaFotos == idGaleria && f.IsPrincipal);

            if (fotoPrincipal == null)
            {
                return NotFound(new { message = "Esta galeria não possui foto principal definida" });
            }

            var fotoDto = _mapper.Map<FotoDto>(fotoPrincipal);
            return Ok(fotoDto);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao buscar foto principal da galeria ID: {IdGaleria}", idGaleria);
            return StatusCode(500, new { message = "Erro ao buscar foto principal", error = ex.Message });
        }
    }
}
