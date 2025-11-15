using Auria.Data.Entities;

namespace Auria.Data.Repositories.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByLoginAsync(string login);
}
