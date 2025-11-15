using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Auria.Data.Context;
using Auria.Data.Repositories.Interfaces;
using Serilog;

namespace Auria.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AuriaDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger _logger;

    public Repository(AuriaDbContext context, ILogger logger)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _logger = logger;
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        try
        {
            return await _dbSet.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar entidade por ID: {Id}", id);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            return await _dbSet.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar todas as entidades");
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        try
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar entidades com filtro");
            throw;
        }
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        try
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar primeira entidade com filtro");
            throw;
        }
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            _logger.Information("Entidade adicionada com sucesso: {Entity}", typeof(T).Name);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao adicionar entidade");
            throw;
        }
    }

    public virtual async Task UpdateAsync(T entity)
    {
        try
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            _logger.Information("Entidade atualizada com sucesso: {Entity}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao atualizar entidade");
            throw;
        }
    }

    public virtual async Task DeleteAsync(int id)
    {
        try
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                _logger.Information("Entidade removida com sucesso: {Entity} - ID: {Id}", typeof(T).Name, id);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao deletar entidade com ID: {Id}", id);
            throw;
        }
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        try
        {
            return await _dbSet.AnyAsync(predicate);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao verificar existência de entidade");
            throw;
        }
    }
}
