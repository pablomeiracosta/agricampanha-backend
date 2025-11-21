using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auria.Bll.Services.Interfaces;
using Auria.Dto.Cotacoes;
using Auria.Structure;

namespace Auria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CotacoesController : ControllerBase
{
    private readonly ICotacaoService _cotacaoService;
    private readonly AuriaContext _context;

    public CotacoesController(ICotacaoService cotacaoService, AuriaContext context)
    {
        _cotacaoService = cotacaoService;
        _context = context;
    }

    /// <summary>
    /// Obtém todas as cotações
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<CotacaoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CotacaoDto>>> GetAll()
    {
        try
        {
            var cotacoes = await _cotacaoService.GetAllAsync();
            return Ok(cotacoes);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao buscar todas as cotações");
            return StatusCode(500, "Erro ao buscar cotações");
        }
    }

    /// <summary>
    /// Obtém uma cotação por ID
    /// </summary>
    /// <param name="id">ID da cotação</param>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CotacaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CotacaoDto>> GetById(int id)
    {
        try
        {
            var cotacao = await _cotacaoService.GetByIdAsync(id);

            if (cotacao == null)
            {
                return NotFound($"Cotação com ID {id} não encontrada");
            }

            return Ok(cotacao);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao buscar cotação por ID: {Id}", id);
            return StatusCode(500, "Erro ao buscar cotação");
        }
    }

    /// <summary>
    /// Obtém a cotação mais recente com indicadores de tendência
    /// </summary>
    /// <remarks>
    /// Retorna a cotação mais recente comparada com a segunda cotação mais recente.
    /// Os indicadores de tendência podem ser:
    /// - "up": valor aumentou
    /// - "down": valor diminuiu
    /// - "stable": valor está igual
    /// </remarks>
    [HttpGet("recente")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CotacaoComTendenciaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CotacaoComTendenciaDto>> GetMostRecentWithTrend()
    {
        try
        {
            var cotacao = await _cotacaoService.GetMostRecentWithTrendAsync();

            if (cotacao == null)
            {
                return NotFound("Nenhuma cotação encontrada");
            }

            return Ok(cotacao);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao buscar cotação mais recente com tendências");
            return StatusCode(500, "Erro ao buscar cotação mais recente");
        }
    }

    /// <summary>
    /// Cria uma nova cotação
    /// </summary>
    /// <param name="cotacaoDto">Dados da cotação</param>
    [HttpPost]
    [ProducesResponseType(typeof(CotacaoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CotacaoDto>> Create([FromBody] CotacaoCreateDto cotacaoDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cotacao = await _cotacaoService.CreateAsync(cotacaoDto);
            return CreatedAtAction(nameof(GetById), new { id = cotacao.IdCotacao }, cotacao);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao criar cotação");
            return StatusCode(500, "Erro ao criar cotação");
        }
    }

    /// <summary>
    /// Atualiza uma cotação existente
    /// </summary>
    /// <param name="id">ID da cotação</param>
    /// <param name="cotacaoDto">Dados atualizados</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CotacaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CotacaoDto>> Update(int id, [FromBody] CotacaoUpdateDto cotacaoDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cotacao = await _cotacaoService.UpdateAsync(id, cotacaoDto);
            return Ok(cotacao);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("não encontrada"))
            {
                return NotFound(ex.Message);
            }

            _context.Log.Error(ex, "Erro ao atualizar cotação: ID {Id}", id);
            return StatusCode(500, "Erro ao atualizar cotação");
        }
    }

    /// <summary>
    /// Deleta uma cotação
    /// </summary>
    /// <param name="id">ID da cotação</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _cotacaoService.DeleteAsync(id);

            if (!result)
            {
                return NotFound($"Cotação com ID {id} não encontrada");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao deletar cotação: ID {Id}", id);
            return StatusCode(500, "Erro ao deletar cotação");
        }
    }
}
