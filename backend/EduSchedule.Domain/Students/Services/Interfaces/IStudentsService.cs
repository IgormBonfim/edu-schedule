using EduSchedule.Domain.Students.Entities;

namespace EduSchedule.Domain.Students.Services.Interfaces;

public interface IStudentsService
{
    Task<Student> GetValidWithEventsAsync(int id, CancellationToken cancellationToken = default);
}
