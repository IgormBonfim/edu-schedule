namespace EduSchedule.Application.Generics.Dtos.Responses
{
    public record PaginatedResponse<T>(IEnumerable<T> Values, int TotalValues, int CurrentPage, int TotalPages);
}