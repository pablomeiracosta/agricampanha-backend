namespace Auria.Dto.Cotacoes;

public class CotacaoComTendenciaDto
{
    public int IdCotacao { get; set; }
    public decimal Soja { get; set; }
    public decimal Arroz { get; set; }
    public decimal Milho { get; set; }
    public DateTime DataCadastro { get; set; }

    // Indicadores de tendência: "up" (subiu), "down" (desceu), "stable" (igual)
    public string TendenciaSoja { get; set; } = "stable";
    public string TendenciaArroz { get; set; } = "stable";
    public string TendenciaMilho { get; set; } = "stable";
}
