using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auria.Data.Entities;

[Table("AGRICAMPANHA_NOTICIA")]
public class Noticia
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Subtitulo { get; set; } = string.Empty;

    [Required]
    [ForeignKey("Categoria")]
    public int CategoriaId { get; set; }

    // Relacionamento com Categoria
    public virtual CategoriaNoticia? Categoria { get; set; }

    [Required]
    public DateTime DataNoticia { get; set; }

    [Required]
    [MaxLength(100)]
    public string Fonte { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Texto { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImagemUrl { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public DateTime? DataAtualizacao { get; set; }
}
