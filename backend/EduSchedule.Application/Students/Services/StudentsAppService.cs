using EduSchedule.Application.Generics.Dtos.Responses;
using EduSchedule.Application.Students.Dtos.Requests;
using EduSchedule.Application.Students.Dtos.Responses;
using EduSchedule.Application.Students.Services.Interfaces;
using EduSchedule.Domain.Students.Entities;
using EduSchedule.Domain.Students.Repositories;
using Microsoft.Extensions.Logging;

namespace EduSchedule.Application.Students.Services;

public class StudentsAppService : IStudentsAppService
{
    private readonly IStudentsRepository _studentsRepository;
    private readonly ILogger<StudentsAppService> _logger;

    public StudentsAppService(IStudentsRepository studentsRepository, ILogger<StudentsAppService> logger)
    {
        _studentsRepository = studentsRepository;
        _logger = logger;
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
