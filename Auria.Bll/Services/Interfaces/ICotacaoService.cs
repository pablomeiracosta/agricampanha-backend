using Auria.Dto;
using Auria.Dto.Cotacoes;

namespace Auria.Bll.Services.Interfaces;

public interface ICotacaoService
{
    Task<IEnumerable<CotacaoDto>> GetAllAsync();
    Task<CotacaoDto?> GetByIdAsync(int id);
    Task<CotacaoComTendenciaDto?> GetMostRecentWithTrendAsync();
    Task<CotacaoDto> CreateAsync(CotacaoCreateDto cotacaoDto);
    Task<CotacaoDto> UpdateAsync(int id, CotacaoUpdateDto cotacaoDto);
    Task<bool> DeleteAsync(int id);
}
