using Microsoft.AspNetCore.Identity;
using Task4Itransition.Application.Abstracts.Processor;
using Task4Itransition.Application.Abstracts.Services;
using Task4Itransition.Application.DTO.Auth;
using Task4Itransition.Application.Exceptions;
using Task4Itransition.Domain.Abstracts.Repository;
using Task4Itransition.Domain.Entities;

namespace Task4Itransition.Application.Services;

public class AuthService(UserManager<User> userManager, IUserRepository userRepository, IAuthTokenProcessor authTokenProcessor) : IAuthService
{
    private readonly IAuthTokenProcessor _authTokenProcessor = authTokenProcessor;
    private readonly UserManager<User> _userManager = userManager;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task RegisterAsync(RegisterRequest registerRequest)
    {
        var userExists = await _userManager.FindByEmailAsync(registerRequest.Email) != null;
        if (userExists)
        {
            throw new UserAlreadyExistsExceptions(registerRequest.Email);
        }

        var result = await _userManager.CreateAsync(User.Create(registerRequest.Email, registerRequest.UserName), registerRequest.Password);
        if (!result.Succeeded)
        {
            throw new RegistrationFailedException(result.Errors.Select(x => x.Description));
        }
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            throw new LoginFailedException(loginRequest.Email);
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            throw new UserBlockException("User account is locked.");
        }

        var (jwtToken, expiresAtUtc, refreshToken, refreshTokenExpiresAtUtc) = GenerateTokensAndUpdateUser(user);

        await _userManager.UpdateAsync(user);

        return new AuthResponse(jwtToken, expiresAtUtc, refreshToken, refreshTokenExpiresAtUtc);
    }

    public async Task LogoutAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return;
        }

        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
    
        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTimeAsUtc = null;
            await _userManager.UpdateAsync(user);
        }
    }

    public async Task<AuthResponse> RefreshTokenAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new RefreshTokenException("Refresh token is missing.");
        }

        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken) ?? throw new RefreshTokenException("Unable to retrieve user for refresh token.");
        if (user.RefreshTokenExpiryTimeAsUtc < DateTime.UtcNow)
        {
            throw new RefreshTokenException("Refresh token is expired.");
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            throw new UserBlockException("User account is locked.");
        }

        var (jwtToken, expiresAtUtc, newRefreshToken, refreshTokenExpiresAtUtc) = GenerateTokensAndUpdateUser(user);

        await _userManager.UpdateAsync(user);

        return new AuthResponse(jwtToken, expiresAtUtc, newRefreshToken, refreshTokenExpiresAtUtc);
    }

    private (string jwtToken, DateTime expiresAtUtc, string refreshToken, DateTime refreshTokenExpiresAtUtc) GenerateTokensAndUpdateUser(User user)
    {
        var (jwtToken, expiresAtUtc) = _authTokenProcessor.GenerateJwtToken(user);
        
        var newRefreshToken = _authTokenProcessor.GenerateRefreshToken();
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTimeAsUtc = refreshTokenExpiresAtUtc;

        return (jwtToken, expiresAtUtc, newRefreshToken, refreshTokenExpiresAtUtc);
    }
}