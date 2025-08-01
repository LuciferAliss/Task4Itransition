using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task4Itransition.Application.Abstracts.Processor;
using Task4Itransition.Application.Abstracts.Services;
using Task4Itransition.Application.DTO.Auth;

namespace Task4Itransition.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService accountService, IAuthTokenProcessor _authTokenProcessor) : ControllerBase
{
    private const string ACCESS_TOKEN_COOKIE_NAME = "ACCESS_TOKEN";
    private const string REFRESH_TOKEN_COOKIE_NAME = "REFRESH_TOKEN";

    private readonly IAuthService _authService = accountService;
    private readonly IAuthTokenProcessor _authTokenProcessor = _authTokenProcessor; 

    [HttpPost("register")]
    public async Task<IResult> Register(RegisterRequest request)
    {
        await _authService.RegisterAsync(request);

        return Results.Ok();
    }

    [HttpPost("login")]
    public async Task<IResult> Login(LoginRequest request)
    {
        var authResponse = await _authService.LoginAsync(request);

        _authTokenProcessor.WriteAuthTokenCookie(ACCESS_TOKEN_COOKIE_NAME, authResponse.JwtToken, authResponse.ExpiresAtUtc);
        _authTokenProcessor.WriteAuthTokenCookie(REFRESH_TOKEN_COOKIE_NAME, authResponse.RefreshToken, authResponse.RefreshTokenExpiresAtUtc);

        return Results.Ok();
    }

    [HttpPost("refresh")]
    public async Task<IResult> RefreshToken()
    {
        string? refreshToken = HttpContext.Request.Cookies[REFRESH_TOKEN_COOKIE_NAME];
        var authResponse = await _authService.RefreshTokenAsync(refreshToken);

        _authTokenProcessor.WriteAuthTokenCookie(ACCESS_TOKEN_COOKIE_NAME, authResponse.JwtToken, authResponse.ExpiresAtUtc);
        _authTokenProcessor.WriteAuthTokenCookie(REFRESH_TOKEN_COOKIE_NAME, authResponse.RefreshToken, authResponse.RefreshTokenExpiresAtUtc);

        return Results.Ok();
    }

    [HttpPost("logout")]
    public async Task<IResult> Logout()
    {
        var refreshToken = Request.Cookies[REFRESH_TOKEN_COOKIE_NAME];
        await _authService.LogoutAsync(refreshToken);

        _authTokenProcessor.DeleteAuthTokenCookie(ACCESS_TOKEN_COOKIE_NAME);
        _authTokenProcessor.DeleteAuthTokenCookie(REFRESH_TOKEN_COOKIE_NAME);

        return Results.Ok();
    }
}
