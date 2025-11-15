using AutoMapper;
using Auria.Bll.Services.Interfaces;
using Auria.Data.Entities;
using Auria.Data.Repositories.Interfaces;
using Auria.Dto.Categorias;
using Auria.Structure;
using Serilog;

namespace Auria.Bll.Services;

public class CategoriaNoticiaService : ICategoriaNoticiaService
{
    private readonly ICategoriaNoticiaRepository _categoriaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public CategoriaNoticiaService(
        ICategoriaNoticiaRepository categoriaRepository,
        AuriaContext context)
    {
        _categoriaRepository = categoriaRepository;
        _mapper = context.Mapper ?? throw new InvalidOperationException("Mapper não configurado");
        _logger = context.Log;
    }

    public async Task<IEnumerable<CategoriaNoticiaDto>> GetAllAsync()
    {
        try
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoriaNoticiaDto>>(categorias);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar todas as categorias");
            throw;
        }
    }

    public async Task<IEnumerable<CategoriaNoticiaDto>> GetAtivosAsync()
    {
        try
        {
            var categorias = await _categoriaRepository.GetAtivosAsync();
            return _mapper.Map<IEnumerable<CategoriaNoticiaDto>>(categorias);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar categorias ativas");
            throw;
        }
    }

    public async Task<CategoriaNoticiaDto?> GetByIdAsync(int id)
    {
        try
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            return categoria == null ? null : _mapper.Map<CategoriaNoticiaDto>(categoria);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar categoria por ID: {Id}", id);
            throw;
        }
    }

    public async Task<CategoriaNoticiaDto> CreateAsync(CategoriaNoticiaCreateDto categoriaDto)
    {
        try
        {
            _logger.Information("Criando nova categoria: {Nome}", categoriaDto.Nome);

            // Verifica se já existe uma categoria com o mesmo nome
            var categoriaExistente = await _categoriaRepository.GetByNomeAsync(categoriaDto.Nome);
            if (categoriaExistente != null)
            {
                throw new Exception($"Já existe uma categoria com o nome '{categoriaDto.Nome}'");
            }

            var categoria = _mapper.Map<CategoriaNoticia>(categoriaDto);
            var categoriaCreated = await _categoriaRepository.AddAsync(categoria);

            _logger.Information("Categoria criada com sucesso: ID {Id}", categoriaCreated.Id);
            return _mapper.Map<CategoriaNoticiaDto>(categoriaCreated);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao criar categoria");
            throw;
        }
    }

    public async Task<CategoriaNoticiaDto> UpdateAsync(CategoriaNoticiaUpdateDto categoriaDto)
    {
        try
        {
            _logger.Information("Atualizando categoria: ID {Id}", categoriaDto.Id);

            var categoriaExistente = await _categoriaRepository.GetByIdAsync(categoriaDto.Id);
            if (categoriaExistente == null)
            {
                throw new Exception($"Categoria com ID {categoriaDto.Id} não encontrada");
            }

            // Verifica se o novo nome já está sendo usado por outra categoria
            var categoriaComMesmoNome = await _categoriaRepository.GetByNomeAsync(categoriaDto.Nome);
            if (categoriaComMesmoNome != null && categoriaComMesmoNome.Id != categoriaDto.Id)
            {
                throw new Exception($"Já existe outra categoria com o nome '{categoriaDto.Nome}'");
            }

            categoriaExistente.Nome = categoriaDto.Nome;
            categoriaExistente.Descricao = categoriaDto.Descricao;
            categoriaExistente.Ativo = categoriaDto.Ativo;

            await _categoriaRepository.UpdateAsync(categoriaExistente);
            _logger.Information("Categoria atualizada com sucesso: ID {Id}", categoriaDto.Id);

            return _mapper.Map<CategoriaNoticiaDto>(categoriaExistente);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao atualizar categoria: ID {Id}", categoriaDto.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            _logger.Information("Deletando categoria: ID {Id}", id);

            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
            {
                _logger.Warning("Categoria não encontrada: ID {Id}", id);
                return false;
            }

            // Verificar se existem notícias associadas
            if (categoria.Noticias != null && categoria.Noticias.Any())
            {
                throw new Exception($"Não é possível deletar a categoria pois existem {categoria.Noticias.Count} notícias associadas");
            }

            await _categoriaRepository.DeleteAsync(id);
            _logger.Information("Categoria deletada com sucesso: ID {Id}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao deletar categoria: ID {Id}", id);
            throw;
        }
    }
}
