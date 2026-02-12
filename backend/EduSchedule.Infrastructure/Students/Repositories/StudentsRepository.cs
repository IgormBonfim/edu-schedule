using EduSchedule.Domain.Students.Entities;
using EduSchedule.Domain.Students.Repositories;
using EduSchedule.Infrastructure.Database;
using EduSchedule.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EduSchedule.Infrastructure.Students.Repositories
{
    public class StudentsRepository : EFRepository<Student>, IStudentsRepository
    {
        public StudentsRepository(EduScheduleDbContext context) : base(context) { }

        public async Task<List<Student>> GetStudentsBatchAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            return await  _dbSet.Include(s => s.Events)
                .Where(s => s.IsActive)
                .OrderBy(s => s.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<Student?> GetWithEventsAsync(string externaId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Include(x => x.Events).FirstOrDefaultAsync(x => x.ExternalId == externaId, cancellationToken);
        }

        public async Task<Student?> GetWithEventsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Include(x => x.Events).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}