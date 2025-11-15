using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auria.Data.Entities;

[Table("AGRICAMPANHA_PROJETO")]
public class Projeto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "TEXT")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// ID da galeria de fotos relacionada
    /// </summary>
    public int? IdGaleriaFotos { get; set; }

    // Relacionamento com GaleriaFotos
    [ForeignKey("IdGaleriaFotos")]
    public virtual GaleriaFotos? GaleriaFotos { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public DateTime? DataAtualizacao { get; set; }

    /// <summary>
    /// Indica se o projeto está ativo/publicado
    /// </summary>
    public bool Ativo { get; set; } = true;
}
