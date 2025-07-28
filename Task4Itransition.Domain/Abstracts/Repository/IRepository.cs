using System.Linq.Expressions;

namespace Task4Itransition.Domain.Abstracts.Repository;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task DeleteAsync(T entity);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<int> SaveChangesAsync();
    Task UpdateAsync(T entity);
}