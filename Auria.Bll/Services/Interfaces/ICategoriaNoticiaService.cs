using Auria.Dto.Categorias;

namespace Auria.Bll.Services.Interfaces;

public interface ICategoriaNoticiaService
{
    Task<IEnumerable<CategoriaNoticiaDto>> GetAllAsync();
    Task<IEnumerable<CategoriaNoticiaDto>> GetAtivosAsync();
    Task<CategoriaNoticiaDto?> GetByIdAsync(int id);
    Task<CategoriaNoticiaDto> CreateAsync(CategoriaNoticiaCreateDto categoriaDto);
    Task<CategoriaNoticiaDto> UpdateAsync(CategoriaNoticiaUpdateDto categoriaDto);
    Task<bool> DeleteAsync(int id);
}
