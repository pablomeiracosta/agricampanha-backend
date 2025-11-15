using Microsoft.EntityFrameworkCore;
using Auria.Data.Context;
using Auria.Data.Entities;
using Auria.Data.Repositories.Interfaces;
using Serilog;

namespace Auria.Data.Repositories;

public class CategoriaNoticiaRepository : Repository<CategoriaNoticia>, ICategoriaNoticiaRepository
{
    public CategoriaNoticiaRepository(AuriaDbContext context, ILogger logger) : base(context, logger)
    {
    }

    public async Task<IEnumerable<CategoriaNoticia>> GetAtivosAsync()
    {
        try
        {
            return await _dbSet
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar categorias ativas");
            throw;
        }
    }

    public async Task<CategoriaNoticia?> GetByNomeAsync(string nome)
    {
        try
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Nome.ToLower() == nome.ToLower());
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar categoria por nome: {Nome}", nome);
            throw;
        }
    }
}
