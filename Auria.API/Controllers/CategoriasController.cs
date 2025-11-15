using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auria.Bll.Services.Interfaces;
using Auria.Dto.Categorias;
using Auria.Structure;

namespace Auria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaNoticiaService _categoriaService;
    private readonly AuriaContext _context;

    public CategoriasController(ICategoriaNoticiaService categoriaService, AuriaContext context)
    {
        _categoriaService = categoriaService;
        _context = context;
    }

    /// <summary>
    /// Obtém todas as categorias
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<CategoriaNoticiaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaNoticiaDto>>> GetAll()
    {
        try
        {
            var categorias = await _categoriaService.GetAllAsync();
            return Ok(categorias);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao buscar todas as categorias");
            return StatusCode(500, "Erro ao buscar categorias");
        }
    }

    /// <summary>
    /// Obtém apenas as categorias ativas
    /// </summary>
    [HttpGet("ativas")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<CategoriaNoticiaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaNoticiaDto>>> GetAtivas()
    {
        try
        {
            var categorias = await _categoriaService.GetAtivosAsync();
            return Ok(categorias);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao buscar categorias ativas");
            return StatusCode(500, "Erro ao buscar categorias ativas");
        }
    }

    /// <summary>
    /// Obtém uma categoria por ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CategoriaNoticiaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaNoticiaDto>> GetById(int id)
    {
        try
        {
            var categoria = await _categoriaService.GetByIdAsync(id);
            if (categoria == null)
            {
                return NotFound($"Categoria com ID {id} não encontrada");
            }
            return Ok(categoria);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao buscar categoria por ID: {Id}", id);
            return StatusCode(500, "Erro ao buscar categoria");
        }
    }

    /// <summary>
    /// Cria uma nova categoria
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaNoticiaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoriaNoticiaDto>> Create([FromBody] CategoriaNoticiaCreateDto categoriaDto)
    {
        try
        {
            var categoria = await _categoriaService.CreateAsync(categoriaDto);
            return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Já existe"))
            {
                return BadRequest(ex.Message);
            }

            _context.Log.Error(ex, "Erro ao criar categoria");
            return StatusCode(500, "Erro ao criar categoria");
        }
    }

    /// <summary>
    /// Atualiza uma categoria existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CategoriaNoticiaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoriaNoticiaDto>> Update(int id, [FromBody] CategoriaNoticiaUpdateDto categoriaDto)
    {
        try
        {
            if (id != categoriaDto.Id)
            {
                return BadRequest("ID da URL não corresponde ao ID do objeto");
            }

            var categoria = await _categoriaService.UpdateAsync(categoriaDto);
            return Ok(categoria);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("não encontrada"))
            {
                return NotFound(ex.Message);
            }

            if (ex.Message.Contains("Já existe"))
            {
                return BadRequest(ex.Message);
            }

            _context.Log.Error(ex, "Erro ao atualizar categoria: {Id}", id);
            return StatusCode(500, "Erro ao atualizar categoria");
        }
    }

    /// <summary>
    /// Deleta uma categoria
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var result = await _categoriaService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Categoria com ID {id} não encontrada");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("notícias associadas"))
            {
                return BadRequest(ex.Message);
            }

            _context.Log.Error(ex, "Erro ao deletar categoria: {Id}", id);
            return StatusCode(500, "Erro ao deletar categoria");
        }
    }
}
