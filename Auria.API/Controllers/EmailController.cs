using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auria.Bll.Services.Interfaces;
using Auria.Dto.Email;
using Auria.Structure;

namespace Auria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly AuriaContext _context;

    public EmailController(IEmailService emailService, AuriaContext context)
    {
        _emailService = emailService;
        _context = context;
    }

    /// <summary>
    /// Envia um email para um destinatário
    /// </summary>
    /// <param name="emailDto">Dados do email</param>
    /// <returns>Status do envio</returns>
    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendEmail([FromBody] EmailSendDto emailDto)
    {
        try
        {
            _context.Log.Information("Solicitação de envio de email para: {To}", emailDto.To);

            var result = await _emailService.SendEmailAsync(
                emailDto.To,
                emailDto.Subject,
                emailDto.Body,
                emailDto.IsHtml
            );

            if (result)
            {
                _context.Log.Information("Email enviado com sucesso para: {To}", emailDto.To);
                return Ok(new { message = "Email enviado com sucesso", to = emailDto.To });
            }

            _context.Log.Warning("Falha ao enviar email para: {To}", emailDto.To);
            return StatusCode(500, new { message = "Falha ao enviar email" });
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao enviar email para: {To}", emailDto.To);
            return StatusCode(500, new { message = "Erro ao enviar email", error = ex.Message });
        }
    }

    /// <summary>
    /// Envia um email para múltiplos destinatários
    /// </summary>
    /// <param name="emailDto">Dados do email</param>
    /// <returns>Status do envio</returns>
    [HttpPost("send/multiple")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendEmailMultiple([FromBody] EmailSendMultipleDto emailDto)
    {
        try
        {
            _context.Log.Information("Solicitação de envio de email para {Count} destinatários", emailDto.ToList.Count);

            // Validar emails
            var invalidEmails = emailDto.ToList.Where(email => !IsValidEmail(email)).ToList();
            if (invalidEmails.Any())
            {
                return BadRequest(new { message = "Emails inválidos encontrados", invalidEmails });
            }

            var result = await _emailService.SendEmailAsync(
                emailDto.ToList,
                emailDto.Subject,
                emailDto.Body,
                emailDto.IsHtml
            );

            if (result)
            {
                _context.Log.Information("Email enviado com sucesso para {Count} destinatários", emailDto.ToList.Count);
                return Ok(new
                {
                    message = "Email enviado com sucesso",
                    recipients = emailDto.ToList.Count,
                    toList = emailDto.ToList
                });
            }

            _context.Log.Warning("Falha ao enviar email para múltiplos destinatários");
            return StatusCode(500, new { message = "Falha ao enviar email" });
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao enviar email para múltiplos destinatários");
            return StatusCode(500, new { message = "Erro ao enviar email", error = ex.Message });
        }
    }

    /// <summary>
    /// Envia um email de teste
    /// </summary>
    /// <param name="to">Email de destino</param>
    /// <returns>Status do envio</returns>
    [HttpPost("test")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendTestEmail([FromQuery] string to)
    {
        try
        {
            if (string.IsNullOrEmpty(to) || !IsValidEmail(to))
            {
                return BadRequest(new { message = "Email inválido" });
            }

            _context.Log.Information("Enviando email de teste para: {To}", to);

            var htmlBody = @"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #4CAF50;'>Email de Teste - Auria API</h2>
                    <p>Este é um email de teste enviado pela API do sistema Auria.</p>
                    <p>Se você recebeu este email, significa que a integração com o Gmail está funcionando corretamente!</p>
                    <hr>
                    <p style='color: #666; font-size: 12px;'>
                        Enviado em: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + @"
                    </p>
                </body>
                </html>";

            var result = await _emailService.SendEmailAsync(to, "Teste - Auria API", htmlBody, true);

            if (result)
            {
                return Ok(new { message = "Email de teste enviado com sucesso", to });
            }

            return StatusCode(500, new { message = "Falha ao enviar email de teste" });
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao enviar email de teste");
            return StatusCode(500, new { message = "Erro ao enviar email de teste", error = ex.Message });
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
