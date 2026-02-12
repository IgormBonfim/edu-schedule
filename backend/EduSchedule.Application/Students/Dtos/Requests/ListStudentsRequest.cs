using EduSchedule.Application.Generics.Dtos.Requests;

namespace EduSchedule.Application.Students.Dtos.Requests;

public class ListStudentsRequest : PaginatedRequest
{
    public int? Id { get; set; }
    public string? ExternalId { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
}
