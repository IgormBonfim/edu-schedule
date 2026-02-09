namespace EduSchedule.Domain.Integrations.Models;

public record UsersDeltaResult(
    IEnumerable<string> ChangedIds, 
    string NextDeltaToken
);