using System.ComponentModel.DataAnnotations;

namespace Auria.Dto.Categorias;

public class CategoriaNoticiaCreateDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }

    public bool Ativo { get; set; } = true;
}
