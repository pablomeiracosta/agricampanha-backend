using FluentValidation;
using Auria.Dto.Noticias;

namespace Auria.API.Validators;

public class NoticiaUpdateValidator : AbstractValidator<NoticiaUpdateDto>
{
    public NoticiaUpdateValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID inválido");

        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Título é obrigatório")
            .MaximumLength(200).WithMessage("Título deve ter no máximo 200 caracteres");

        RuleFor(x => x.Subtitulo)
            .NotEmpty().WithMessage("Subtítulo é obrigatório")
            .MaximumLength(300).WithMessage("Subtítulo deve ter no máximo 300 caracteres");

        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("Categoria é obrigatória");

        RuleFor(x => x.DataNoticia)
            .NotEmpty().WithMessage("Data da notícia é obrigatória")
            .LessThanOrEqualTo(DateTime.Now.AddDays(1)).WithMessage("Data da notícia não pode ser futura");

        RuleFor(x => x.Fonte)
            .NotEmpty().WithMessage("Fonte é obrigatória")
            .MaximumLength(100).WithMessage("Fonte deve ter no máximo 100 caracteres");

        RuleFor(x => x.Texto)
            .NotEmpty().WithMessage("Texto é obrigatório");

        RuleFor(x => x.ImagemUrl)
            .MaximumLength(500).WithMessage("URL da imagem deve ter no máximo 500 caracteres");
    }
}
