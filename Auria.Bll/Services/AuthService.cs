using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Auria.Bll.Services.Interfaces;
using Auria.Data.Repositories.Interfaces;
using Auria.Dto.Login;
using Auria.Structure;
using BCrypt.Net;
using Serilog;

namespace Auria.Bll.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly AuriaContext _context;
    private readonly ILogger _logger;

    public AuthService(IUsuarioRepository usuarioRepository, AuriaContext context)
    {
        _usuarioRepository = usuarioRepository;
        _context = context;
        _logger = context.Log;
    }

    public async Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto request)
    {
        try
        {
            _logger.Information("Tentativa de autenticação para o usuário: {Login}", request.Login);

            var usuario = await _usuarioRepository.GetByLoginAsync(request.Login);

            if (usuario == null)
            {
                _logger.Warning("Usuário não encontrado: {Login}", request.Login);
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Usuário ou senha inválidos"
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            {
                _logger.Warning("Senha inválida para o usuário: {Login}", request.Login);
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Usuário ou senha inválidos"
                };
            }

            var token = GenerateJwtToken(usuario.Id, usuario.Nome, usuario.Login);

            _logger.Information("Autenticação bem-sucedida para o usuário: {Login}", request.Login);

            return new LoginResponseDto
            {
                Success = true,
                Token = token,
                Message = "Login realizado com sucesso",
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Login = usuario.Login
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao autenticar usuário: {Login}", request.Login);
            return new LoginResponseDto
            {
                Success = false,
                Message = "Erro ao processar autenticação"
            };
        }
    }

    public string GenerateJwtToken(int usuarioId, string nome, string login)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_context.Settings.Jwt.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                new Claim(ClaimTypes.Name, nome),
                new Claim("login", login)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_context.Settings.Jwt.ExpirationMinutes),
            Issuer = _context.Settings.Jwt.Issuer,
            Audience = _context.Settings.Jwt.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
