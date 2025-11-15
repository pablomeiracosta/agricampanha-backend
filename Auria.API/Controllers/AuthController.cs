using Microsoft.AspNetCore.Mvc;
using Auria.Bll.Services.Interfaces;
using Auria.Dto.Login;
using Auria.Structure;

namespace Auria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly AuriaContext _context;

    public AuthController(IAuthService authService, AuriaContext context)
    {
        _authService = authService;
        _context = context;
    }

    /// <summary>
    /// Realiza o login do usuário
    /// </summary>
    /// <param name="request">Dados de login (Login e Senha)</param>
    /// <returns>Token JWT e informações do usuário</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            _context.Log.Information("Tentativa de login para usuário: {Login}", request.Login);

            var response = await _authService.AuthenticateAsync(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _context.Log.Error(ex, "Erro ao processar login");
            return StatusCode(500, new LoginResponseDto
            {
                Success = false,
                Message = "Erro interno ao processar login"
            });
        }
    }
}
