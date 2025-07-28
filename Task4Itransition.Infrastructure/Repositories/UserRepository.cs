using Microsoft.EntityFrameworkCore;
using Task4Itransition.Domain.Abstracts.Repository;
using Task4Itransition.Domain.Entities;
using Task4Itransition.Infrastructure.Data;

namespace Task4Itransition.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}