using Auria.Data.Entities;

namespace Auria.Data.Repositories.Interfaces;

public interface ICotacaoRepository : IRepository<Cotacao>
{
    Task<IEnumerable<Cotacao>> GetMostRecentAsync(int count);
}
