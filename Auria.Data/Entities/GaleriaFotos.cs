using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auria.Data.Entities;

[Table("AGRICAMPANHA_GALERIA_FOTOS")]
public class GaleriaFotos
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Tipo de referência da galeria: 1-Notícias, 2-Projetos, etc
    /// </summary>
    [Required]
    public int IdReferencia { get; set; }

    /// <summary>
    /// ID do registro relacionado (ex: ID da notícia, ID do projeto, etc)
    /// </summary>
    public int? IdRegistroRelacionado { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public DateTime? DataAtualizacao { get; set; }

    // Relacionamento com Fotos
    public virtual ICollection<Foto> Fotos { get; set; } = new List<Foto>();
}
