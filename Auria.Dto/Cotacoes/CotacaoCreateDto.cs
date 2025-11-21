using System.ComponentModel.DataAnnotations;

namespace Auria.Dto.Cotacoes;

public class CotacaoCreateDto
{
    [Required(ErrorMessage = "O valor da Soja é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor da Soja deve ser maior que zero")]
    public decimal Soja { get; set; }

    [Required(ErrorMessage = "O valor do Arroz é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor do Arroz deve ser maior que zero")]
    public decimal Arroz { get; set; }

    [Required(ErrorMessage = "O valor do Milho é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor do Milho deve ser maior que zero")]
    public decimal Milho { get; set; }
}
