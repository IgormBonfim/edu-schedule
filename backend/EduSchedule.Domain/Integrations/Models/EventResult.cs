namespace EduSchedule.Domain.Integrations.Models
{
    public record EventResult(
        string Id, 
        string Subject, 
        DateTimeOffset? Start, 
        DateTimeOffset? End, 
        bool IsDeleted
    );
}