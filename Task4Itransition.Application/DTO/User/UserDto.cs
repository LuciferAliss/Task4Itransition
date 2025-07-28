namespace Task4Itransition.Application.DTO.User;

public record UserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime? LastLogin { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime CreatedAt { get; set; }
}