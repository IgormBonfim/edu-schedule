using EduSchedule.Domain.Integrations.Models;

namespace EduSchedule.Domain.Integrations.Services.Interfaces
{
    public interface IGraphService
    {
        Task<DeltaResults<UserResult>> GetUsersDeltaAsync(string? deltaLink = null, int top = 999, CancellationToken cancellationToken = default);
        Task<UserResult?> GetUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<DeltaResults<EventResult>> GetUserEventsDeltaAsync(string studentId, string? deltaLink, int top = 999, CancellationToken cancellationToken = default);
    }
}