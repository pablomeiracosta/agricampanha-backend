using Microsoft.EntityFrameworkCore;
using Auria.Data.Entities;

namespace Auria.Data.Context;

public class AuriaDbContext : DbContext
{
    public AuriaDbContext(DbContextOptions<AuriaDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Noticia> Noticias { get; set; }
    public DbSet<CategoriaNoticia> CategoriasNoticias { get; set; }
    public DbSet<GaleriaFotos> GaleriasFotos { get; set; }
    public DbSet<Foto> Fotos { get; set; }
    public DbSet<Projeto> Projetos { get; set; }
    public DbSet<Cotacao> Cotacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações adicionais podem ser feitas aqui
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Login)
            .IsUnique();

        // Configurar relacionamento entre Noticia e CategoriaNoticia
        modelBuilder.Entity<Noticia>()
            .HasOne(n => n.Categoria)
            .WithMany(c => c.Noticias)
            .HasForeignKey(n => n.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Noticia>()
            .HasIndex(n => n.DataNoticia);

        modelBuilder.Entity<Noticia>()
            .HasIndex(n => n.CategoriaId);

        modelBuilder.Entity<CategoriaNoticia>()
            .HasIndex(c => c.Nome)
            .IsUnique();

        // Configurar relacionamento entre Foto e GaleriaFotos
        modelBuilder.Entity<Foto>()
            .HasOne(f => f.GaleriaFotos)
            .WithMany(g => g.Fotos)
            .HasForeignKey(f => f.IdGaleriaFotos)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Foto>()
            .HasIndex(f => f.IdGaleriaFotos);

        modelBuilder.Entity<GaleriaFotos>()
            .HasIndex(g => g.IdReferencia);

        // Configurar relacionamento entre Projeto e GaleriaFotos
        modelBuilder.Entity<Projeto>()
            .HasOne(p => p.GaleriaFotos)
            .WithMany()
            .HasForeignKey(p => p.IdGaleriaFotos)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Projeto>()
            .HasIndex(p => p.Ativo);

        modelBuilder.Entity<Projeto>()
            .HasIndex(p => p.DataCriacao);

        // Configurar índice para Cotacao
        modelBuilder.Entity<Cotacao>()
            .HasIndex(c => c.DataCadastro);
    }
}
