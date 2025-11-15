using Microsoft.EntityFrameworkCore;
using Auria.Data.Context;
using Auria.Data.Entities;
using Auria.Data.Repositories.Interfaces;
using Serilog;

namespace Auria.Data.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AuriaDbContext context, ILogger logger) : base(context, logger)
    {
    }

    public async Task<Usuario?> GetByLoginAsync(string login)
    {
        try
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Login == login && u.Ativo);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar usuário por login: {Login}", login);
            throw;
        }
    }
}
