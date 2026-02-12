using EduSchedule.Application.Generics.Dtos.Responses;
using EduSchedule.Application.Students.Dtos.Requests;
using EduSchedule.Application.Students.Dtos.Responses;
using EduSchedule.Application.Students.Services.Interfaces;
using EduSchedule.Domain.Students.Entities;
using EduSchedule.Domain.Students.Repositories;
using EduSchedule.Domain.Students.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace EduSchedule.Application.Students.Services;

public class StudentsAppService : IStudentsAppService
{
    private readonly IStudentsService _studentsService;
    private readonly IStudentsRepository _studentsRepository;
    private readonly ILogger<StudentsAppService> _logger;

    public StudentsAppService(IStudentsService studentsService, IStudentsRepository studentsRepository, ILogger<StudentsAppService> logger)
    {
        _studentsService = studentsService;
        _studentsRepository = studentsRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<EventResponse>> GetStudentEventsAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _studentsService.GetValidWithEventsAsync(id, cancellationToken);

        var values = student.Events.Select(x => new EventResponse(x.Id, x.ExternalId, x.Subject, x.StartTime, x.EndTime));

        return values;
    }

    public async Task<PaginatedResponse<StudentResponse>> GetStudentsAsync(ListStudentsRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = QueryBuilder(request);
            var results = await _studentsRepository.ToListPaginatedAsync(query, request.ItemsPerPage, request.Page, cancellationToken);

            var students = results.Values.Select(x => new StudentResponse(x.Id, x.ExternalId, x.DisplayName, x.Email));

            return new PaginatedResponse<StudentResponse>(students, results.TotalValues, results.CurrentPage, results.TotalPages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro ao Listar os Estudantes. Request: {@request}", request);
            throw;
        }
    }

    private IQueryable<Student> QueryBuilder(ListStudentsRequest request)
    {
        var query = _studentsRepository.Query();

        if (request.Id is not null)
            query = query.Where(x => x.Id == request.Id);

        if (request.ExternalId is not null)
            query = query.Where(x => x.ExternalId == request.ExternalId);

        if (request.Email is not null)
            query = query.Where(x => x.Email.StartsWith(request.Email));

        if (request.DisplayName is not null)
            query = query.Where(x => x.DisplayName.StartsWith(request.DisplayName));

        return query;
    }
}
