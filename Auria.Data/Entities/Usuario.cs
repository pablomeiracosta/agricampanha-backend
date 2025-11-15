using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auria.Data.Entities;

[Table("AGRICAMPANHA_USUARIO")]
public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Login { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string SenhaHash { get; set; } = string.Empty;

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public bool Ativo { get; set; } = true;
}
