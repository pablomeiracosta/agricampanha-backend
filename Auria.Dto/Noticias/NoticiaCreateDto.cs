using System.ComponentModel.DataAnnotations;

namespace Auria.Dto.Noticias;

public class NoticiaCreateDto
{
    [Required(ErrorMessage = "O título é obrigatório")]
    public string Titulo { get; set; } = string.Empty;

    public string Subtitulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A categoria é obrigatória")]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "A data da notícia é obrigatória")]
    public DateTime DataNoticia { get; set; }

    public string Fonte { get; set; } = string.Empty;

    [Required(ErrorMessage = "O texto é obrigatório")]
    public string Texto { get; set; } = string.Empty;

    public string? ImagemUrl { get; set; }
}
