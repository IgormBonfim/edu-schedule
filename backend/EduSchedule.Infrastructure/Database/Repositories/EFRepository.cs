using System.Linq.Expressions;
using EduSchedule.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EduSchedule.Infrastructure.Database.Repositories
{
    public class EFRepository<T> : IRepository<T> where T : class
    {
        private readonly EduScheduleDbContext _context;
        public readonly DbSet<T> _dbSet;

        public EFRepository(EduScheduleDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(id, cancellationToken);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(expression).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default)
        {
            var entityEntry = await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entityEntry.Entity;
        }

        public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            var entityEntry = _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entityEntry.Entity;
        }

        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<IEnumerable<T>> ListarAsync(IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            return await query.ToListAsync(cancellationToken);
        }
    }
}