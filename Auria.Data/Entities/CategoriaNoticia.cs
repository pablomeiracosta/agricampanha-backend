namespace Auria.Data.Entities;

public class CategoriaNoticia
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.Now;

    // Relacionamento com Notícias
    public virtual ICollection<Noticia> Noticias { get; set; } = new List<Noticia>();
}
