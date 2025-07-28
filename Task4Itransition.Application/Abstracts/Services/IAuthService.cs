using Task4Itransition.Application.DTO.Auth;

namespace Task4Itransition.Application.Abstracts.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
    Task LogoutAsync(string? refreshToken);
    Task<AuthResponse> RefreshTokenAsync(string? refreshToken);
    Task RegisterAsync(RegisterRequest registerRequest);
}
