using Auria.Data.Entities;

namespace Auria.Data.Repositories.Interfaces;

public interface ICategoriaNoticiaRepository : IRepository<CategoriaNoticia>
{
    Task<IEnumerable<CategoriaNoticia>> GetAtivosAsync();
    Task<CategoriaNoticia?> GetByNomeAsync(string nome);
}
