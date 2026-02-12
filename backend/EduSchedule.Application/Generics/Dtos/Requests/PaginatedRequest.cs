namespace EduSchedule.Application.Generics.Dtos.Requests;

public class PaginatedRequest
{
    public int Page { get; set; } = 1;
    public int ItemsPerPage { get; set; } = 10;
}
