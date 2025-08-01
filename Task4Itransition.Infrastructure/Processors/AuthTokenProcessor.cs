using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Task4Itransition.Application.Abstracts.Processor;
using Task4Itransition.Domain.Entities;
using Task4Itransition.Infrastructure.Options;

namespace Task4Itransition.Domain.Processors;

public class AuthTokenProcessor(IOptions<JwtOptions> jwtOptions, IHttpContextAccessor httpContextAccessor) : IAuthTokenProcessor
{
    private const int SIZE_REFRESH_TOKEN = 64;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private SigningCredentials GetSigningCredentials()
    {
        return new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret)), SecurityAlgorithms.HmacSha256);
    }

    private IEnumerable<Claim> GenerateClaimsForUser(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Name, user.ToString())
        };

        return claims;
    }

    public (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(User user)
    {
        var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationTimeInMinutes);

        return (new JwtSecurityTokenHandler()
            .WriteToken(new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: GenerateClaimsForUser(user),
                expires: expires,
                signingCredentials: GetSigningCredentials())
            ), expires);
    }

    public string GenerateRefreshToken()
    {
        byte[] rt = new byte[SIZE_REFRESH_TOKEN];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(rt);
        return Convert.ToBase64String(rt);
    }

    public void WriteAuthTokenCookie(string CookieName, string token, DateTime expiration)
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Append(CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Expires = expiration,
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.None 
        });
    }
    
    public void DeleteAuthTokenCookie(string cookieName)
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, "", new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(-1),
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.None 
        });
    }
}
