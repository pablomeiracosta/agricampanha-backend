using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auria.Data.Entities;

[Table("AGRICAMPANHA_FOTO")]
public class Foto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [ForeignKey("GaleriaFotos")]
    public int IdGaleriaFotos { get; set; }

    // Relacionamento com GaleriaFotos
    public virtual GaleriaFotos? GaleriaFotos { get; set; }

    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NomeArquivo { get; set; }

    [MaxLength(300)]
    public string? Legenda { get; set; }

    public long? Tamanho { get; set; }

    public int? Ordem { get; set; }

    /// <summary>
    /// Indica se esta é a foto principal da galeria
    /// </summary>
    public bool IsPrincipal { get; set; } = false;

    public DateTime DataUpload { get; set; } = DateTime.Now;
}
