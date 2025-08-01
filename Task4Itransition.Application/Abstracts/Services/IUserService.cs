using Task4Itransition.Application.DTO.User;
using Task4Itransition.Domain.Entities;

namespace Task4Itransition.Application.Abstracts.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync(string currentUserId);
    Task<UserDto> GetUserAsync(string id);
    Task DeleteUserAsync(Guid id, string currentUserId);
    Task BlockUserAsync(Guid id, string currentUserId);
    Task UnblockUserAsync(Guid id, string currentUserId);
}
