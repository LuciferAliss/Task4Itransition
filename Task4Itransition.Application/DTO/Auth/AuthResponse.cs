using System;

namespace Task4Itransition.Application.DTO.Auth;

public record AuthResponse(string JwtToken, DateTime ExpiresAtUtc, string RefreshToken, DateTime RefreshTokenExpiresAtUtc);
