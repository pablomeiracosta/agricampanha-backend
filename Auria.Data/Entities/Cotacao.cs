using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auria.Data.Entities;

[Table("AGRICAMPANHA_COTACOES")]
public class Cotacao
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdCotacao { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Soja { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Arroz { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Milho { get; set; }

    [Required]
    public DateTime DataCadastro { get; set; } = DateTime.Now;
}
