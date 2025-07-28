using Task4Itransition.Domain.Entities;

namespace Task4Itransition.Domain.Abstracts.Repository;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
}