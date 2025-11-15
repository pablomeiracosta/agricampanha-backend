using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auria.Bll.Services.Interfaces;
using Auria.Dto;
using Auria.Dto.Noticias;
using Auria.Structure;

namespace Auria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NoticiasController : ControllerBase
{
    private readonly INoticiaService _noticiaService;
    private readonly AuriaContext _context;

    public NoticiasController(INoticiaService noticiaService, AuriaContext context)
    {
        _noticiaService = noticiaService;
        _context = context;
    }

    /// <summary>
    /// Obtém todas as notícias
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<NoticiaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NoticiaDto>>> GetAll()
    {
        try
        {
            var noticias = await _noticiaService.GetAllAsync();
            return Ok(noticias);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao buscar todas as notícias");
            return StatusCode(500, "Erro ao buscar notícias");
        }
    }

    /// <summary>
    /// Obtém notícias paginadas
    /// </summary>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    [HttpGet("paginadas")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedResponseDto<NoticiaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedResponseDto<NoticiaDto>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (pageNumber < 1)
            {
                return BadRequest("O número da página deve ser maior ou igual a 1");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest("O tamanho da página deve estar entre 1 e 100");
            }

            var result = await _noticiaService.GetPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao buscar notícias paginadas: Página {PageNumber}, Tamanho {PageSize}", pageNumber, pageSize);
            return StatusCode(500, "Erro ao buscar notícias paginadas");
        }
    }

    /// <summary>
    /// Obtém uma notícia por ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(NoticiaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NoticiaDto>> GetById(int id)
    {
        try
        {
            var noticia = await _noticiaService.GetByIdAsync(id);
            if (noticia == null)
            {
                return NotFound($"Notícia com ID {id} não encontrada");
            }
            return Ok(noticia);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao buscar notícia por ID: {Id}", id);
            return StatusCode(500, "Erro ao buscar notícia");
        }
    }

    /// <summary>
    /// Obtém notícias por categoria
    /// </summary>
    [HttpGet("categoria/{categoriaId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<NoticiaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NoticiaDto>>> GetByCategoria(int categoriaId)
    {
        try
        {
            var noticias = await _noticiaService.GetByCategoriaIdAsync(categoriaId);
            return Ok(noticias);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao buscar notícias por categoria: {CategoriaId}", categoriaId);
            return StatusCode(500, "Erro ao buscar notícias por categoria");
        }
    }

    /// <summary>
    /// Cria uma nova notícia
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NoticiaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NoticiaDto>> Create([FromBody] NoticiaCreateDto noticiaDto)
    {
        try
        {
            var noticia = await _noticiaService.CreateAsync(noticiaDto);
            return CreatedAtAction(nameof(GetById), new { id = noticia.Id }, noticia);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao criar notícia");
            return StatusCode(500, "Erro ao criar notícia");
        }
    }

    /// <summary>
    /// Atualiza uma notícia existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(NoticiaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NoticiaDto>> Update(int id, [FromBody] NoticiaUpdateDto noticiaDto)
    {
        try
        {
            if (id != noticiaDto.Id)
            {
                return BadRequest("ID da URL não corresponde ao ID do objeto");
            }

            var noticia = await _noticiaService.UpdateAsync(noticiaDto);
            return Ok(noticia);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("não encontrada"))
            {
                return NotFound(ex.Message);
            }

            _context.Log.Error(ex, "Erro ao atualizar notícia: {Id}", id);
            return StatusCode(500, "Erro ao atualizar notícia");
        }
    }

    /// <summary>
    /// Deleta uma notícia
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var result = await _noticiaService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Notícia com ID {id} não encontrada");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao deletar notícia: {Id}", id);
            return StatusCode(500, "Erro ao deletar notícia");
        }
    }
}
