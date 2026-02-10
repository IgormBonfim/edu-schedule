namespace EduSchedule.Domain.Integrations.Models;

public record UsersDeltaResult(
    IEnumerable<UserResult> ChangedUsers, 
    string? NextDeltaToken,
    string? NextDeltaLink
);