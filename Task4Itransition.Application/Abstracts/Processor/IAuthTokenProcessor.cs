using Task4Itransition.Domain.Entities;

namespace Task4Itransition.Application.Abstracts.Processor;

public interface IAuthTokenProcessor
{
    (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(User user);
    string GenerateRefreshToken();
    void WriteAuthTokenCookie(string CookieName, string token, DateTime expiration);
    void DeleteAuthTokenCookie(string CookieName);
}
