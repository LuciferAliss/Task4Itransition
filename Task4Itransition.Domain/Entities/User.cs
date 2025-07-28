using Microsoft.AspNetCore.Identity;

namespace Task4Itransition.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTimeAsUtc { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; }

    public static User Create(string email, string username)
    {
        return new User
        {
            Email = email,
            UserName = username,
            LockoutEnabled = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}