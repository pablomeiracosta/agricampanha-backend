using System.ComponentModel.DataAnnotations;

namespace Auria.Dto.GaleriaFotos;

public class GaleriaFotosDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int IdReferencia { get; set; }
    public int? IdRegistroRelacionado { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public List<FotoDto>? Fotos { get; set; }
}

public class GaleriaFotosCreateDto
{
    [Required(ErrorMessage = "O título é obrigatório")]
    [MaxLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "IdReferencia é obrigatório")]
    public int IdReferencia { get; set; }

    public int? IdRegistroRelacionado { get; set; }
}

public class GaleriaFotosUpdateDto
{
    [Required(ErrorMessage = "O título é obrigatório")]
    [MaxLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "IdReferencia é obrigatório")]
    public int IdReferencia { get; set; }

    public int? IdRegistroRelacionado { get; set; }
}

public class FotoDto
{
    public int Id { get; set; }
    public int IdGaleriaFotos { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? NomeArquivo { get; set; }
    public string? Legenda { get; set; }
    public long? Tamanho { get; set; }
    public int? Ordem { get; set; }
    public bool IsPrincipal { get; set; }
    public DateTime DataUpload { get; set; }
}

public class FotoCreateDto
{
    [Required(ErrorMessage = "IdGaleriaFotos é obrigatório")]
    public int IdGaleriaFotos { get; set; }

    [Required(ErrorMessage = "URL é obrigatória")]
    [MaxLength(500, ErrorMessage = "URL deve ter no máximo 500 caracteres")]
    public string Url { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "NomeArquivo deve ter no máximo 200 caracteres")]
    public string? NomeArquivo { get; set; }

    [MaxLength(300, ErrorMessage = "Legenda deve ter no máximo 300 caracteres")]
    public string? Legenda { get; set; }

    public long? Tamanho { get; set; }

    public int? Ordem { get; set; }
}

public class FotoUpdateDto
{
    [MaxLength(300, ErrorMessage = "Legenda deve ter no máximo 300 caracteres")]
    public string? Legenda { get; set; }

    public int? Ordem { get; set; }
}
