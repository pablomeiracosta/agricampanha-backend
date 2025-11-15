using Microsoft.EntityFrameworkCore;
using Auria.Data.Context;
using Auria.Data.Entities;
using Auria.Data.Repositories.Interfaces;
using Serilog;

namespace Auria.Data.Repositories;

public class NoticiaRepository : Repository<Noticia>, INoticiaRepository
{
    public NoticiaRepository(AuriaDbContext context, ILogger logger) : base(context, logger)
    {
    }

    public async Task<IEnumerable<Noticia>> GetByCategoriaIdAsync(int categoriaId)
    {
        try
        {
            return await _dbSet
                .Include(n => n.Categoria)
                .Where(n => n.CategoriaId == categoriaId)
                .OrderByDescending(n => n.DataNoticia)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar notícias por categoria: {CategoriaId}", categoriaId);
            throw;
        }
    }

    public async Task<IEnumerable<Noticia>> GetByDataRangeAsync(DateTime dataInicio, DateTime dataFim)
    {
        try
        {
            return await _dbSet
                .Where(n => n.DataNoticia >= dataInicio && n.DataNoticia <= dataFim)
                .OrderByDescending(n => n.DataNoticia)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar notícias por range de data: {DataInicio} - {DataFim}", dataInicio, dataFim);
            throw;
        }
    }

    public async Task<(IEnumerable<Noticia> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        try
        {
            var totalCount = await _dbSet.CountAsync();

            var items = await _dbSet
                .OrderByDescending(n => n.DataNoticia)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar notícias paginadas: Página {PageNumber}, Tamanho {PageSize}", pageNumber, pageSize);
            throw;
        }
    }
}
