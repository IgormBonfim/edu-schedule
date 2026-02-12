using EduSchedule.Domain.Repositories;
using EduSchedule.Domain.Students.Entities;

namespace EduSchedule.Domain.Students.Repositories
{
    public interface IStudentsRepository : IRepository<Student>
    {
        Task<List<Student>> GetStudentsBatchAsync(int skip, int take, CancellationToken cancellationToken = default);
        Task<Student?> GetWithEventsAsync(string externaId, CancellationToken cancellationToken = default);
    }
}