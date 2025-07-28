using Task4Itransition.Application.DTO.User;
using Task4Itransition.Domain.Entities;

namespace Task4Itransition.Application.Abstracts.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync(string refreshToken);
    Task DeleteUserAsync(Guid id, string refreshToken);
    Task BlockUserAsync(Guid id, string refreshToken);
    Task UnblockUserAsync(Guid id, string refreshToken);
    Task CheckBlockUserAsync(string refreshToken);
}
