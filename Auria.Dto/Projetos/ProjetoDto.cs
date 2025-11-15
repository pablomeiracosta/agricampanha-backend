using System.ComponentModel.DataAnnotations;
using Auria.Dto.GaleriaFotos;

namespace Auria.Dto.Projetos;

public class ProjetoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int? IdGaleriaFotos { get; set; }
    public GaleriaFotosDto? GaleriaFotos { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool Ativo { get; set; }
}

public class ProjetoCreateDto
{
    [Required(ErrorMessage = "O título é obrigatório")]
    [MaxLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória")]
    public string Descricao { get; set; } = string.Empty;

    public int? IdGaleriaFotos { get; set; }

    public bool Ativo { get; set; } = true;
}

public class ProjetoUpdateDto
{
    [Required(ErrorMessage = "O título é obrigatório")]
    [MaxLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória")]
    public string Descricao { get; set; } = string.Empty;

    public int? IdGaleriaFotos { get; set; }
}
