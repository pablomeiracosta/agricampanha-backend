using Microsoft.EntityFrameworkCore;
using Auria.Data.Context;
using Auria.Data.Entities;
using Auria.Data.Repositories.Interfaces;
using Serilog;

namespace Auria.Data.Repositories;

public class CotacaoRepository : Repository<Cotacao>, ICotacaoRepository
{
    public CotacaoRepository(AuriaDbContext context, ILogger logger) : base(context, logger)
    {
    }

    public async Task<IEnumerable<Cotacao>> GetMostRecentAsync(int count)
    {
        try
        {
            return await _dbSet
                .OrderByDescending(c => c.DataCadastro)
                .Take(count)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar cotações mais recentes");
            throw;
        }
    }
}
