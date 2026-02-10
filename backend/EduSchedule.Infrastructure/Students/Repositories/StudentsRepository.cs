using EduSchedule.Domain.Students.Entities;
using EduSchedule.Domain.Students.Repositories;
using EduSchedule.Infrastructure.Database;
using EduSchedule.Infrastructure.Database.Repositories;

namespace EduSchedule.Infrastructure.Students.Repositories
{
    public class StudentsRepository : EFRepository<Student>, IStudentsRepository
    {
        public StudentsRepository(EduScheduleDbContext context) : base(context) { }
    }
}