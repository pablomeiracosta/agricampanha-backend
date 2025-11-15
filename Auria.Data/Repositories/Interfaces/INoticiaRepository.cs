using Auria.Data.Entities;

namespace Auria.Data.Repositories.Interfaces;

public interface INoticiaRepository : IRepository<Noticia>
{
    Task<IEnumerable<Noticia>> GetByCategoriaIdAsync(int categoriaId);
    Task<IEnumerable<Noticia>> GetByDataRangeAsync(DateTime dataInicio, DateTime dataFim);
    Task<(IEnumerable<Noticia> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
}
