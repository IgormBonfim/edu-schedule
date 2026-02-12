using EduSchedule.Application.Generics.Dtos.Responses;
using EduSchedule.Application.Students.Dtos.Requests;
using EduSchedule.Application.Students.Dtos.Responses;

namespace EduSchedule.Application.Students.Services.Interfaces;

public interface IStudentsAppService
{
    Task<PaginatedResponse<StudentResponse>> GetStudentsAsync(ListStudentsRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventResponse>> GetStudentEventsAsync(int id, CancellationToken cancellationToken = default);
}
