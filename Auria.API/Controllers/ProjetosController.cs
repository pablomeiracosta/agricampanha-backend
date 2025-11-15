using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Auria.Data.Context;
using Auria.Data.Entities;
using Auria.Dto;
using Auria.Dto.Projetos;
using Auria.Structure;

namespace Auria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjetosController : ControllerBase
{
    private readonly AuriaDbContext _context;
    private readonly IMapper _mapper;
    private readonly AuriaContext _auriaContext;

    public ProjetosController(AuriaDbContext context, IMapper mapper, AuriaContext auriaContext)
    {
        _context = context;
        _mapper = mapper;
        _auriaContext = auriaContext;
    }

    /// <summary>
    /// Retorna todos os projetos com paginação
    /// </summary>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    /// <param name="apenasAtivos">Filtrar apenas projetos ativos (padrão: true)</param>
    /// <returns>Lista paginada de projetos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponseDto<ProjetoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponseDto<ProjetoDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool apenasAtivos = true)
    {
        try
        {
            _auriaContext.Log.Information("Buscando projetos - Página: {PageNumber}, Tamanho: {PageSize}, Apenas Ativos: {ApenasAtivos}",
                pageNumber, pageSize, apenasAtivos);

            var query = _context.Projetos
                .Include(p => p.GaleriaFotos)
                    .ThenInclude(g => g.Fotos)
                .OrderByDescending(p => p.DataCriacao)
                .AsQueryable();

            if (apenasAtivos)
            {
                query = query.Where(p => p.Ativo);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var projetos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var projetosDto = _mapper.Map<List<ProjetoDto>>(projetos);

            var response = new PaginatedResponseDto<ProjetoDto>(
                projetosDto,
                totalItems,
                pageNumber,
                pageSize
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao buscar projetos");
            return StatusCode(500, new { message = "Erro ao buscar projetos", error = ex.Message });
        }
    }

    /// <summary>
    /// Retorna um projeto específico por ID
    /// </summary>
    /// <param name="id">ID do projeto</param>
    /// <returns>Projeto encontrado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProjetoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjetoDto>> GetById(int id)
    {
        try
        {
            _auriaContext.Log.Information("Buscando projeto ID: {Id}", id);

            var projeto = await _context.Projetos
                .Include(p => p.GaleriaFotos)
                    .ThenInclude(g => g.Fotos.OrderBy(f => f.Ordem))
                .FirstOrDefaultAsync(p => p.Id == id);

            if (projeto == null)
            {
                _auriaContext.Log.Warning("Projeto não encontrado: {Id}", id);
                return NotFound(new { message = "Projeto não encontrado" });
            }

            var projetoDto = _mapper.Map<ProjetoDto>(projeto);
            return Ok(projetoDto);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao buscar projeto ID: {Id}", id);
            return StatusCode(500, new { message = "Erro ao buscar projeto", error = ex.Message });
        }
    }

    /// <summary>
    /// Retorna apenas projetos ativos
    /// </summary>
    /// <returns>Lista de projetos ativos</returns>
    [HttpGet("ativos")]
    [ProducesResponseType(typeof(List<ProjetoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProjetoDto>>> GetAtivos()
    {
        try
        {
            _auriaContext.Log.Information("Buscando projetos ativos");

            var projetos = await _context.Projetos
                .Include(p => p.GaleriaFotos)
                    .ThenInclude(g => g.Fotos.OrderBy(f => f.Ordem))
                .Where(p => p.Ativo)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            var projetosDto = _mapper.Map<List<ProjetoDto>>(projetos);
            return Ok(projetosDto);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao buscar projetos ativos");
            return StatusCode(500, new { message = "Erro ao buscar projetos ativos", error = ex.Message });
        }
    }

    /// <summary>
    /// Cria um novo projeto
    /// </summary>
    /// <param name="projetoDto">Dados do projeto</param>
    /// <returns>Projeto criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProjetoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjetoDto>> Create([FromBody] ProjetoCreateDto projetoDto)
    {
        try
        {
            _auriaContext.Log.Information("Criando novo projeto: {Titulo}", projetoDto.Titulo);

            // Validar se a galeria de fotos existe, se fornecida
            if (projetoDto.IdGaleriaFotos.HasValue)
            {
                var galeriaExiste = await _context.GaleriasFotos
                    .AnyAsync(g => g.Id == projetoDto.IdGaleriaFotos.Value);

                if (!galeriaExiste)
                {
                    return BadRequest(new { message = "Galeria de fotos não encontrada" });
                }
            }

            var projeto = _mapper.Map<Projeto>(projetoDto);
            projeto.DataCriacao = DateTime.Now;

            _context.Projetos.Add(projeto);
            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Projeto criado com sucesso. ID: {Id}", projeto.Id);

            var projetoResultado = await _context.Projetos
                .Include(p => p.GaleriaFotos)
                    .ThenInclude(g => g.Fotos)
                .FirstOrDefaultAsync(p => p.Id == projeto.Id);

            var resultado = _mapper.Map<ProjetoDto>(projetoResultado);
            return CreatedAtAction(nameof(GetById), new { id = projeto.Id }, resultado);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao criar projeto");
            return StatusCode(500, new { message = "Erro ao criar projeto", error = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza um projeto existente
    /// </summary>
    /// <param name="id">ID do projeto</param>
    /// <param name="projetoDto">Dados atualizados</param>
    /// <returns>Projeto atualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProjetoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjetoDto>> Update(int id, [FromBody] ProjetoUpdateDto projetoDto)
    {
        try
        {
            _auriaContext.Log.Information("Atualizando projeto ID: {Id}", id);

            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto == null)
            {
                _auriaContext.Log.Warning("Projeto não encontrado para atualização: {Id}", id);
                return NotFound(new { message = "Projeto não encontrado" });
            }

            // Validar se a galeria de fotos existe, se fornecida
            if (projetoDto.IdGaleriaFotos.HasValue)
            {
                var galeriaExiste = await _context.GaleriasFotos
                    .AnyAsync(g => g.Id == projetoDto.IdGaleriaFotos.Value);

                if (!galeriaExiste)
                {
                    return BadRequest(new { message = "Galeria de fotos não encontrada" });
                }
            }

            _mapper.Map(projetoDto, projeto);
            projeto.DataAtualizacao = DateTime.Now;

            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Projeto atualizado com sucesso: {Id}", id);

            var projetoAtualizado = await _context.Projetos
                .Include(p => p.GaleriaFotos)
                    .ThenInclude(g => g.Fotos)
                .FirstOrDefaultAsync(p => p.Id == id);

            var resultado = _mapper.Map<ProjetoDto>(projetoAtualizado);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao atualizar projeto ID: {Id}", id);
            return StatusCode(500, new { message = "Erro ao atualizar projeto", error = ex.Message });
        }
    }

    /// <summary>
    /// Deleta um projeto
    /// </summary>
    /// <param name="id">ID do projeto</param>
    /// <returns>Status da operação</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _auriaContext.Log.Information("Deletando projeto ID: {Id}", id);

            var projeto = await _context.Projetos.FindAsync(id);

            if (projeto == null)
            {
                _auriaContext.Log.Warning("Projeto não encontrado para deleção: {Id}", id);
                return NotFound(new { message = "Projeto não encontrado" });
            }

            _context.Projetos.Remove(projeto);
            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Projeto deletado com sucesso: {Id}", id);
            return Ok(new { message = "Projeto deletado com sucesso" });
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao deletar projeto ID: {Id}", id);
            return StatusCode(500, new { message = "Erro ao deletar projeto", error = ex.Message });
        }
    }

    /// <summary>
    /// Alterna o status ativo/inativo de um projeto
    /// </summary>
    /// <param name="id">ID do projeto</param>
    /// <returns>Projeto com status atualizado</returns>
    [HttpPatch("{id}/toggle-ativo")]
    [ProducesResponseType(typeof(ProjetoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjetoDto>> ToggleAtivo(int id)
    {
        try
        {
            _auriaContext.Log.Information("Alternando status ativo do projeto ID: {Id}", id);

            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto == null)
            {
                _auriaContext.Log.Warning("Projeto não encontrado: {Id}", id);
                return NotFound(new { message = "Projeto não encontrado" });
            }

            projeto.Ativo = !projeto.Ativo;
            projeto.DataAtualizacao = DateTime.Now;

            await _context.SaveChangesAsync();

            _auriaContext.Log.Information("Status do projeto alternado. ID: {Id}, Ativo: {Ativo}", id, projeto.Ativo);

            var projetoAtualizado = await _context.Projetos
                .Include(p => p.GaleriaFotos)
                    .ThenInclude(g => g.Fotos)
                .FirstOrDefaultAsync(p => p.Id == id);

            var resultado = _mapper.Map<ProjetoDto>(projetoAtualizado);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _auriaContext.Log.Error(ex, "Erro ao alternar status do projeto ID: {Id}", id);
            return StatusCode(500, new { message = "Erro ao alternar status", error = ex.Message });
        }
    }
}
