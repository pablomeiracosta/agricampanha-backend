using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Auria.Bll.Services.Interfaces;
using Auria.Structure;
using Auria.Structure.Configuration;
using Serilog;
using System.Text;

namespace Auria.Bll.Services;

public class GmailService : IEmailService
{
    private readonly ILogger _logger;
    private readonly GmailSettings _gmailSettings;

    public GmailService(AuriaContext context)
    {
        _logger = context.Log;
        _gmailSettings = context.Settings.Gmail;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        return await SendEmailAsync(new List<string> { to }, subject, body, isHtml);
    }

    public async Task<bool> SendEmailAsync(List<string> toList, string subject, string body, bool isHtml = true)
    {
        try
        {
            _logger.Information("Iniciando envio de email para {Count} destinatário(s)", toList.Count);

            // Criar credencial a partir do JSON
            GoogleCredential credential;

            if (!string.IsNullOrEmpty(_gmailSettings.CredentialsJson))
            {
                // Usando JSON direto da configuração
                credential = GoogleCredential.FromJson(_gmailSettings.CredentialsJson)
                    .CreateScoped(Google.Apis.Gmail.v1.GmailService.Scope.GmailSend);
            }
            else if (!string.IsNullOrEmpty(_gmailSettings.CredentialsPath))
            {
                // Usando arquivo de credenciais
                using var stream = new FileStream(_gmailSettings.CredentialsPath, FileMode.Open, FileAccess.Read);
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Google.Apis.Gmail.v1.GmailService.Scope.GmailSend);
            }
            else
            {
                throw new InvalidOperationException("Nenhuma credencial do Gmail foi configurada");
            }

            // Criar serviço Gmail
            var service = new Google.Apis.Gmail.v1.GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Auria API"
            });

            // Construir email em formato MIME
            var emailMessage = CreateMimeMessage(toList, subject, body, isHtml);

            // Codificar em Base64 URL-safe
            var base64EncodedEmail = Convert.ToBase64String(Encoding.UTF8.GetBytes(emailMessage))
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");

            var message = new Message { Raw = base64EncodedEmail };

            // Enviar email
            var request = service.Users.Messages.Send(message, "me");
            var result = await request.ExecuteAsync();

            _logger.Information("Email enviado com sucesso. ID: {MessageId}", result.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao enviar email");
            return false;
        }
    }

    private string CreateMimeMessage(List<string> toList, string subject, string body, bool isHtml)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"From: {_gmailSettings.FromEmail}");
        sb.AppendLine($"To: {string.Join(", ", toList)}");
        sb.AppendLine($"Subject: {subject}");
        sb.AppendLine("MIME-Version: 1.0");

        if (isHtml)
        {
            sb.AppendLine("Content-Type: text/html; charset=utf-8");
        }
        else
        {
            sb.AppendLine("Content-Type: text/plain; charset=utf-8");
        }

        sb.AppendLine();
        sb.AppendLine(body);

        return sb.ToString();
    }
}
