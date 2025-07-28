using System;

namespace Task4Itransition.Infrastructure.Options;

public class JwtOptions
{
    public const string JWT_OPTIONS_KEY = "JwtOptions";

    public string Secret { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int ExpirationTimeInMinutes { get; set; }
}

