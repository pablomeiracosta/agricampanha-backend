namespace Auria.Bll.Services.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Envia um email usando a Gmail API
    /// </summary>
    /// <param name="to">Destinatário</param>
    /// <param name="subject">Assunto</param>
    /// <param name="body">Corpo do email (suporta HTML)</param>
    /// <param name="isHtml">Se o corpo é HTML (padrão: true)</param>
    /// <returns>True se enviado com sucesso</returns>
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);

    /// <summary>
    /// Envia um email para múltiplos destinatários
    /// </summary>
    /// <param name="toList">Lista de destinatários</param>
    /// <param name="subject">Assunto</param>
    /// <param name="body">Corpo do email (suporta HTML)</param>
    /// <param name="isHtml">Se o corpo é HTML (padrão: true)</param>
    /// <returns>True se enviado com sucesso</returns>
    Task<bool> SendEmailAsync(List<string> toList, string subject, string body, bool isHtml = true);
}
