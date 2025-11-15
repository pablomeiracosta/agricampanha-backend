using Auria.Dto.Categorias;

namespace Auria.Dto.Noticias;

public class NoticiaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Subtitulo { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public CategoriaNoticiaDto? Categoria { get; set; }
    public DateTime DataNoticia { get; set; }
    public string Fonte { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public string? ImagemUrl { get; set; }
}
