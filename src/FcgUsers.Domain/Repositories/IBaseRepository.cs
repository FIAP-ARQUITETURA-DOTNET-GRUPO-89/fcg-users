using System.Linq.Expressions;

namespace FcgUsers.Domain.Interfaces.Repositories;

public interface IBaseRepository<T> where T : class
{
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<T?> GetByIdAsync(Guid id);
    Task<int> SaveChangesAsync();

    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<IReadOnlyList<T>> GetPagedAsync(int pagina, int tamanhoPagina, Expression<Func<T, bool>>? predicate = null);
}
