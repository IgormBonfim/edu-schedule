namespace EduSchedule.Domain.Generics.Models;

public record PaginatedResults<T>(IEnumerable<T> Values, int TotalValues, int CurrentPage, int TotalPages);
