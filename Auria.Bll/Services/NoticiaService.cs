using AutoMapper;
using Auria.Bll.Services.Interfaces;
using Auria.Data.Entities;
using Auria.Data.Repositories.Interfaces;
using Auria.Dto;
using Auria.Dto.Noticias;
using Auria.Structure;
using Serilog;

namespace Auria.Bll.Services;

public class NoticiaService : INoticiaService
{
    private readonly INoticiaRepository _noticiaRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public NoticiaService(
        INoticiaRepository noticiaRepository,
        ICloudinaryService cloudinaryService,
        AuriaContext context)
    {
        _noticiaRepository = noticiaRepository;
        _cloudinaryService = cloudinaryService;
        _mapper = context.Mapper ?? throw new InvalidOperationException("Mapper não configurado");
        _logger = context.Log;
    }

    public async Task<IEnumerable<NoticiaDto>> GetAllAsync()
    {
        try
        {
            var noticias = await _noticiaRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<NoticiaDto>>(noticias);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar todas as notícias");
            throw;
        }
    }

    public async Task<NoticiaDto?> GetByIdAsync(int id)
    {
        try
        {
            var noticia = await _noticiaRepository.GetByIdAsync(id);
            return noticia == null ? null : _mapper.Map<NoticiaDto>(noticia);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar notícia por ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<NoticiaDto>> GetByCategoriaIdAsync(int categoriaId)
    {
        try
        {
            var noticias = await _noticiaRepository.GetByCategoriaIdAsync(categoriaId);
            return _mapper.Map<IEnumerable<NoticiaDto>>(noticias);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar notícias por categoria: {CategoriaId}", categoriaId);
            throw;
        }
    }

    public async Task<PaginatedResponseDto<NoticiaDto>> GetPagedAsync(int pageNumber, int pageSize)
    {
        try
        {
            _logger.Information("Buscando notícias paginadas: Página {PageNumber}, Tamanho {PageSize}", pageNumber, pageSize);

            var (items, totalCount) = await _noticiaRepository.GetPagedAsync(pageNumber, pageSize);
            var noticiasDto = _mapper.Map<IEnumerable<NoticiaDto>>(items);

            return new PaginatedResponseDto<NoticiaDto>(noticiasDto, totalCount, pageNumber, pageSize);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar notícias paginadas: Página {PageNumber}, Tamanho {PageSize}", pageNumber, pageSize);
            throw;
        }
    }

    public async Task<NoticiaDto> CreateAsync(NoticiaCreateDto noticiaDto)
    {
        try
        {
            _logger.Information("Criando nova notícia: {Titulo}", noticiaDto.Titulo);

            var noticia = _mapper.Map<Noticia>(noticiaDto);
            var noticiaCreated = await _noticiaRepository.AddAsync(noticia);
            _logger.Information("Notícia criada com sucesso: ID {Id}", noticiaCreated.Id);

            return _mapper.Map<NoticiaDto>(noticiaCreated);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao criar notícia");
            throw;
        }
    }

    public async Task<NoticiaDto> UpdateAsync(NoticiaUpdateDto noticiaDto)
    {
        try
        {
            _logger.Information("Atualizando notícia: ID {Id}", noticiaDto.Id);

            var noticiaExistente = await _noticiaRepository.GetByIdAsync(noticiaDto.Id);
            if (noticiaExistente == null)
            {
                throw new Exception($"Notícia com ID {noticiaDto.Id} não encontrada");
            }

            // Atualiza os campos
            noticiaExistente.Titulo = noticiaDto.Titulo;
            noticiaExistente.Subtitulo = noticiaDto.Subtitulo;
            noticiaExistente.CategoriaId = noticiaDto.CategoriaId;
            noticiaExistente.DataNoticia = noticiaDto.DataNoticia;
            noticiaExistente.Fonte = noticiaDto.Fonte;
            noticiaExistente.Texto = noticiaDto.Texto;
            noticiaExistente.DataAtualizacao = DateTime.Now;

            // Atualiza ImagemUrl se fornecida
            if (!string.IsNullOrEmpty(noticiaDto.ImagemUrl))
            {
                noticiaExistente.ImagemUrl = noticiaDto.ImagemUrl;
            }

            await _noticiaRepository.UpdateAsync(noticiaExistente);
            _logger.Information("Notícia atualizada com sucesso: ID {Id}", noticiaDto.Id);

            return _mapper.Map<NoticiaDto>(noticiaExistente);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao atualizar notícia: ID {Id}", noticiaDto.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            _logger.Information("Deletando notícia: ID {Id}", id);

            var noticia = await _noticiaRepository.GetByIdAsync(id);
            if (noticia == null)
            {
                _logger.Warning("Notícia não encontrada: ID {Id}", id);
                return false;
            }

            // Deleta a imagem do Cloudinary se existir
            if (!string.IsNullOrEmpty(noticia.ImagemUrl))
            {
                await _cloudinaryService.DeleteImageAsync(noticia.ImagemUrl);
            }

            await _noticiaRepository.DeleteAsync(id);
            _logger.Information("Notícia deletada com sucesso: ID {Id}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao deletar notícia: ID {Id}", id);
            throw;
        }
    }
}
