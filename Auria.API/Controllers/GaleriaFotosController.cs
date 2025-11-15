using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Auria.Data.Context;
using Auria.Data.Entities;
using Auria.Dto;
using Auria.Dto.GaleriaFotos;
using Auria.Structure;

namespace Auria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GaleriaFotosController : ControllerBase
{
    private readonly AuriaDbContext _context;
    private readonly IMapper _mapper;
    private readonly AuriaContext _auriaContext;

    public GaleriaFotosController(AuriaDbContext context, IMapper mapper, AuriaContext auriaContext)
    {
        _context = context;
        _mapper = mapper;
        _auriaContext = auriaContext;
    }

    /// <summary>
    /// Retorna todas as galerias de fotos com paginação
    /// </summary>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    /// <returns>Lista paginada de galerias</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponseDto<GaleriaFotosDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponseDto<GaleriaFotosDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            _auriaContext.Log.Information("Buscando galerias de fotos - Página: {PageNumber}, Tamanho: {PageSize}", pageNumber, pageSize);

            var query = _context.GaleriasFotos
                .Include(g => g.Fotos)
                .OrderByDescending(g => g.DataCriacao)
                .AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var galerias = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var galeriasDto = _mapper.Map<List<GaleriaFotosDto>>(galerias);

            var response = new PaginatedResponseDto<GaleriaFotosDto>(
                galeriasDto,
                totalItems,
                pageNumber,
                pageSize
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao buscar galerias de fotos");
            return StatusCode(500, new { message = "Erro ao buscar galerias", error = ex.Message });
        }
    }

    /// <summary>
    /// Retorna uma galeria específica por ID
    /// </summary>
    /// <param name="id">ID da galeria</param>
    /// <returns>Galeria encontrada</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GaleriaFotosDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GaleriaFotosDto>> GetById(int id)
    {
        try
        {
            _auriaContext.Log.Information("Buscando galeria de fotos ID: {Id}", id);

            var galeria = await _context.GaleriasFotos
                .Include(g => g.Fotos.OrderBy(f => f.Ordem))
                .FirstOrDefaultAsync(g => g.Id == id);

            if (galeria == null)
            {
                _auriaContext.Log.Warning("Galeria de fotos não encontrada: {Id}", id);
                return NotFound(new { message = "Galeria não encontrada" });
            }

            var galeriaDto = _mapper.Map<GaleriaFotosDto>(galeria);
            return Ok(galeriaDto);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao buscar galeria de fotos ID: {Id}", id);
            return StatusCode(500, new { message = "Erro ao buscar galeria", error = ex.Message });
        }
    }

    /// <summary>
    /// Retorna galerias filtradas por IdReferencia
    /// </summary>
    /// <param name="idReferencia">Tipo de referência (1-Notícias, 2-Projetos, etc)</param>
    /// <param name="idRegistroRelacionado">ID do registro relacionado (opcional)</param>
    /// <returns>Lista de galerias filtradas</returns>
    [HttpGet("por-referencia/{idReferencia}")]
    [ProducesResponseType(typeof(List<GaleriaFotosDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GaleriaFotosDto>>> GetByReferencia(
        int idReferencia,
        [FromQuery] int? idRegistroRelacionado = null)
    {
        try
        {
            _auriaContext.Log.Information("Buscando galerias por referência: {IdReferencia}, Registro: {IdRegistro}",
                idReferencia, idRegistroRelacionado);

            var query = _context.GaleriasFotos
                .Include(g => g.Fotos.OrderBy(f => f.Ordem))
                .Where(g => g.IdReferencia == idReferencia);

            if (idRegistroRelacionado.HasValue)
            {
                query = query.Where(g => g.IdRegistroRelacionado == idRegistroRelacionado.Value);
            }

            var galerias = await query
                .OrderByDescending(g => g.DataCriacao)
                .ToListAsync();

            var galeriasDto = _mapper.Map<List<GaleriaFotosDto>>(galerias);
            return Ok(galeriasDto);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao buscar galerias por referência");
            return StatusCode(500, new { message = "Erro ao buscar galerias", error = ex.Message });
        }
    }

    /// <summary>
    /// Cria uma nova galeria de fotos
    /// </summary>
    /// <param name="galeriaDto">Dados da galeria</param>
    /// <returns>Galeria criada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(GaleriaFotosDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GaleriaFotosDto>> Create([FromBody] GaleriaFotosCreateDto galeriaDto)
    {
        try
        {
            _auriaContext.Log.Information("Criando nova galeria de fotos: {Titulo}", galeriaDto.Titulo);

            var galeria = _mapper.Map<GaleriaFotos>(galeriaDto);
            galeria.DataCriacao = DateTime.Now;

            _context.GaleriasFotos.Add(galeria);
            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Galeria criada com sucesso. ID: {Id}", galeria.Id);

            var galeriaResultado = await _context.GaleriasFotos
                .Include(g => g.Fotos)
                .FirstOrDefaultAsync(g => g.Id == galeria.Id);

            var resultado = _mapper.Map<GaleriaFotosDto>(galeriaResultado);
            return CreatedAtAction(nameof(GetById), new { id = galeria.Id }, resultado);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao criar galeria de fotos");
            return StatusCode(500, new { message = "Erro ao criar galeria", error = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza uma galeria existente
    /// </summary>
    /// <param name="id">ID da galeria</param>
    /// <param name="galeriaDto">Dados atualizados</param>
    /// <returns>Galeria atualizada</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(GaleriaFotosDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GaleriaFotosDto>> Update(int id, [FromBody] GaleriaFotosUpdateDto galeriaDto)
    {
        try
        {
            _auriaContext.Log.Information("Atualizando galeria de fotos ID: {Id}", id);

            var galeria = await _context.GaleriasFotos.FindAsync(id);
            if (galeria == null)
            {
                _auriaContext.Log.Warning("Galeria não encontrada para atualização: {Id}", id);
                return NotFound(new { message = "Galeria não encontrada" });
            }

            _mapper.Map(galeriaDto, galeria);
            galeria.DataAtualizacao = DateTime.Now;

            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Galeria atualizada com sucesso: {Id}", id);

            var galeriaAtualizada = await _context.GaleriasFotos
                .Include(g => g.Fotos)
                .FirstOrDefaultAsync(g => g.Id == id);

            var resultado = _mapper.Map<GaleriaFotosDto>(galeriaAtualizada);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao atualizar galeria ID: {Id}", id);
            return StatusCode(500, new { message = "Erro ao atualizar galeria", error = ex.Message });
        }
    }

    /// <summary>
    /// Deleta uma galeria (e todas as suas fotos em cascade)
    /// </summary>
    /// <param name="id">ID da galeria</param>
    /// <returns>Status da operação</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _auriaContext.Log.Information("Deletando galeria de fotos ID: {Id}", id);

            var galeria = await _context.GaleriasFotos
                .Include(g => g.Fotos)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (galeria == null)
            {
                _auriaContext.Log.Warning("Galeria não encontrada para deleção: {Id}", id);
                return NotFound(new { message = "Galeria não encontrada" });
            }

            _context.GaleriasFotos.Remove(galeria);
            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Galeria deletada com sucesso: {Id}", id);
            return Ok(new { message = "Galeria deletada com sucesso" });
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao deletar galeria ID: {Id}", id);
            return StatusCode(500, new { message = "Erro ao deletar galeria", error = ex.Message });
        }
    }

    /// <summary>
    /// Adiciona uma foto à galeria
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <param name="fotoDto">Dados da foto</param>
    /// <returns>Foto adicionada</returns>
    [HttpPost("{idGaleria}/fotos")]
    [ProducesResponseType(typeof(FotoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FotoDto>> AddFoto(int idGaleria, [FromBody] FotoCreateDto fotoDto)
    {
        try
        {
            _auriaContext.Log.Information("Adicionando foto à galeria ID: {IdGaleria}", idGaleria);

            // Verificar se a galeria existe
            var galeria = await _context.GaleriasFotos.FindAsync(idGaleria);
            if (galeria == null)
            {
                _auriaContext.Log.Warning("Galeria não encontrada: {IdGaleria}", idGaleria);
                return NotFound(new { message = "Galeria não encontrada" });
            }

            // Validar se o IdGaleriaFotos do DTO corresponde ao parâmetro
            if (fotoDto.IdGaleriaFotos != idGaleria)
            {
                return BadRequest(new { message = "IdGaleriaFotos não corresponde ao ID da galeria" });
            }

            var foto = _mapper.Map<Foto>(fotoDto);
            foto.DataUpload = DateTime.Now;

            _context.Fotos.Add(foto);
            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Foto adicionada com sucesso. ID: {Id}, Galeria: {IdGaleria}", foto.Id, idGaleria);

            var fotoResultado = await _context.Fotos.FindAsync(foto.Id);
            var resultado = _mapper.Map<FotoDto>(fotoResultado);

            return CreatedAtAction(nameof(GetFoto), new { idGaleria, idFoto = foto.Id }, resultado);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao adicionar foto à galeria ID: {IdGaleria}", idGaleria);
            return StatusCode(500, new { message = "Erro ao adicionar foto", error = ex.Message });
        }
    }

    /// <summary>
    /// Retorna uma foto específica da galeria
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <param name="idFoto">ID da foto</param>
    /// <returns>Foto encontrada</returns>
    [HttpGet("{idGaleria}/fotos/{idFoto}")]
    [ProducesResponseType(typeof(FotoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FotoDto>> GetFoto(int idGaleria, int idFoto)
    {
        try
        {
            _auriaContext.Log.Information("Buscando foto ID: {IdFoto} da galeria ID: {IdGaleria}", idFoto, idGaleria);

            var foto = await _context.Fotos
                .FirstOrDefaultAsync(f => f.Id == idFoto && f.IdGaleriaFotos == idGaleria);

            if (foto == null)
            {
                _auriaContext.Log.Warning("Foto não encontrada: ID {IdFoto}, Galeria {IdGaleria}", idFoto, idGaleria);
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
    /// Atualiza os dados de uma foto (legenda, ordem)
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <param name="idFoto">ID da foto</param>
    /// <param name="fotoDto">Dados atualizados</param>
    /// <returns>Foto atualizada</returns>
    [HttpPut("{idGaleria}/fotos/{idFoto}")]
    [ProducesResponseType(typeof(FotoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FotoDto>> UpdateFoto(int idGaleria, int idFoto, [FromBody] FotoUpdateDto fotoDto)
    {
        try
        {
            _auriaContext.Log.Information("Atualizando foto ID: {IdFoto} da galeria ID: {IdGaleria}", idFoto, idGaleria);

            var foto = await _context.Fotos
                .FirstOrDefaultAsync(f => f.Id == idFoto && f.IdGaleriaFotos == idGaleria);

            if (foto == null)
            {
                _auriaContext.Log.Warning("Foto não encontrada para atualização: ID {IdFoto}, Galeria {IdGaleria}", idFoto, idGaleria);
                return NotFound(new { message = "Foto não encontrada" });
            }

            _mapper.Map(fotoDto, foto);
            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Foto atualizada com sucesso: {IdFoto}", idFoto);

            var fotoAtualizada = await _context.Fotos.FindAsync(idFoto);
            var resultado = _mapper.Map<FotoDto>(fotoAtualizada);

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao atualizar foto ID: {IdFoto}", idFoto);
            return StatusCode(500, new { message = "Erro ao atualizar foto", error = ex.Message });
        }
    }

    /// <summary>
    /// Remove uma foto da galeria
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <param name="idFoto">ID da foto</param>
    /// <returns>Status da operação</returns>
    [HttpDelete("{idGaleria}/fotos/{idFoto}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFoto(int idGaleria, int idFoto)
    {
        try
        {
            _auriaContext.Log.Information("Deletando foto ID: {IdFoto} da galeria ID: {IdGaleria}", idFoto, idGaleria);

            var foto = await _context.Fotos
                .FirstOrDefaultAsync(f => f.Id == idFoto && f.IdGaleriaFotos == idGaleria);

            if (foto == null)
            {
                _auriaContext.Log.Warning("Foto não encontrada para deleção: ID {IdFoto}, Galeria {IdGaleria}", idFoto, idGaleria);
                return NotFound(new { message = "Foto não encontrada" });
            }

            _context.Fotos.Remove(foto);
            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Foto deletada com sucesso: {IdFoto}", idFoto);
            return Ok(new { message = "Foto deletada com sucesso" });
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao deletar foto ID: {IdFoto}", idFoto);
            return StatusCode(500, new { message = "Erro ao deletar foto", error = ex.Message });
        }
    }

    /// <summary>
    /// Define uma foto como principal da galeria
    /// </summary>
    /// <param name="idGaleria">ID da galeria</param>
    /// <param name="idFoto">ID da foto</param>
    /// <returns>Status da operação</returns>
    [HttpPatch("{idGaleria}/fotos/{idFoto}/set-principal")]
    [ProducesResponseType(typeof(FotoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FotoDto>> SetFotoPrincipal(int idGaleria, int idFoto)
    {
        try
        {
            _auriaContext.Log.Information("Definindo foto ID: {IdFoto} como principal da galeria ID: {IdGaleria}", idFoto, idGaleria);

            var foto = await _context.Fotos
                .FirstOrDefaultAsync(f => f.Id == idFoto && f.IdGaleriaFotos == idGaleria);

            if (foto == null)
            {
                _auriaContext.Log.Warning("Foto não encontrada: ID {IdFoto}, Galeria {IdGaleria}", idFoto, idGaleria);
                return NotFound(new { message = "Foto não encontrada" });
            }

            // Remover flag de principal de todas as fotos da galeria
            var fotosDaGaleria = await _context.Fotos
                .Where(f => f.IdGaleriaFotos == idGaleria && f.IsPrincipal)
                .ToListAsync();

            foreach (var f in fotosDaGaleria)
            {
                f.IsPrincipal = false;
            }

            // Definir a foto atual como principal
            foto.IsPrincipal = true;
            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Foto definida como principal: {IdFoto}", idFoto);

            var fotoAtualizada = await _context.Fotos.FindAsync(idFoto);
            var resultado = _mapper.Map<FotoDto>(fotoAtualizada);

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao definir foto principal ID: {IdFoto}", idFoto);
            return StatusCode(500, new { message = "Erro ao definir foto principal", error = ex.Message });
        }
    }
}
