namespace EduSchedule.Application.Students.Dtos.Responses;

public record EventResponse(int Id, string ExternalId, string Subject, DateTime StartTime, DateTime EndTime);