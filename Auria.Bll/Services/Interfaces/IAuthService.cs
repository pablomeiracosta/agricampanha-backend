using Auria.Dto.Login;

namespace Auria.Bll.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto request);
    string GenerateJwtToken(int usuarioId, string nome, string login);
}
