using System.ComponentModel.DataAnnotations;

namespace Auria.Dto.Email;

public class EmailSendDto
{
    [Required(ErrorMessage = "O destinatário é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string To { get; set; } = string.Empty;

    [Required(ErrorMessage = "O assunto é obrigatório")]
    [MaxLength(200, ErrorMessage = "Assunto deve ter no máximo 200 caracteres")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "O corpo do email é obrigatório")]
    public string Body { get; set; } = string.Empty;

    public bool IsHtml { get; set; } = true;
}

public class EmailSendMultipleDto
{
    [Required(ErrorMessage = "Pelo menos um destinatário é obrigatório")]
    [MinLength(1, ErrorMessage = "Informe pelo menos um destinatário")]
    public List<string> ToList { get; set; } = new();

    [Required(ErrorMessage = "O assunto é obrigatório")]
    [MaxLength(200, ErrorMessage = "Assunto deve ter no máximo 200 caracteres")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "O corpo do email é obrigatório")]
    public string Body { get; set; } = string.Empty;

    public bool IsHtml { get; set; } = true;
}
