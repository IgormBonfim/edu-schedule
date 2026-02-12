using System.Linq.Expressions;
using EduSchedule.Domain.Generics.Models;

namespace EduSchedule.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
        Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default);
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        IQueryable<T> Query();
        Task<IEnumerable<T>> ToListAsync(IQueryable<T> query, CancellationToken cancellationToken = default);
        Task<PaginatedResults<T>> ToListPaginatedAsync(IQueryable<T> query, int itemsPerPage, int page, CancellationToken cancellationToken = default);
    }
}