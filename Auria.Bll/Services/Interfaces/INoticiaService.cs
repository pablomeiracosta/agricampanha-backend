using Auria.Dto.Noticias;
using Auria.Dto;

namespace Auria.Bll.Services.Interfaces;

public interface INoticiaService
{
    Task<IEnumerable<NoticiaDto>> GetAllAsync();
    Task<NoticiaDto?> GetByIdAsync(int id);
    Task<IEnumerable<NoticiaDto>> GetByCategoriaIdAsync(int categoriaId);
    Task<PaginatedResponseDto<NoticiaDto>> GetPagedAsync(int pageNumber, int pageSize);
    Task<NoticiaDto> CreateAsync(NoticiaCreateDto noticiaDto);
    Task<NoticiaDto> UpdateAsync(NoticiaUpdateDto noticiaDto);
    Task<bool> DeleteAsync(int id);
}
