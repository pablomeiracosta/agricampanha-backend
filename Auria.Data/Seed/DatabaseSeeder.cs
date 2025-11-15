using Auria.Data.Context;
using Auria.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Auria.Data.Seed;

public class DatabaseSeeder
{
    private readonly AuriaDbContext _context;
    private readonly ILogger _logger;

    public DatabaseSeeder(AuriaDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Garante que o banco de dados está criado
            await _context.Database.MigrateAsync();

            // Seed de usuário inicial
            await SeedUsuarioAsync();

            _logger.Information("Seed do banco de dados concluído com sucesso");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao realizar seed do banco de dados");
            throw;
        }
    }

    private async Task SeedUsuarioAsync()
    {
        if (!await _context.Usuarios.AnyAsync())
        {
            var usuario = new Usuario
            {
                Nome = "Administrador",
                Login = "admin",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // Senha padrão: admin123
                DataCriacao = DateTime.Now,
                Ativo = true
            };

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            _logger.Information("Usuário inicial criado: {Login}", usuario.Login);
        }
    }
}
