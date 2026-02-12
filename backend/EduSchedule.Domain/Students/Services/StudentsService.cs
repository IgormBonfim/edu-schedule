using EduSchedule.Domain.Exceptions;
using EduSchedule.Domain.Students.Entities;
using EduSchedule.Domain.Students.Repositories;
using EduSchedule.Domain.Students.Services.Interfaces;

namespace EduSchedule.Domain.Students.Services;

public class StudentsService : IStudentsService
{
    private readonly IStudentsRepository _studentsRepository;

    public StudentsService(IStudentsRepository studentsRepository)
    {
        _studentsRepository = studentsRepository;
    }
    
    public async Task<Student> GetValidWithEventsAsync(int id, CancellationToken cancellationToken = default)
    {
        Student? student = await _studentsRepository.GetWithEventsAsync(id, cancellationToken);

        if (student is null)
            throw new DomainException("Student not found", "STUDENT_NOT_FOUND");

        return student;
    }
}
