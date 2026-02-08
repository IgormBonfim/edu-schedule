using System.Linq.Expressions;

namespace EduSchedule.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
        Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default);
    }
}