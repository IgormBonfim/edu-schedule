using EduSchedule.Domain.Integrations.Models;

namespace EduSchedule.Domain.Integrations.Services.Interfaces
{
    public interface IGraphService
    {
        Task<UsersDeltaResult> GetUsersDeltaAsync(string? deltaLink = null, int top = 999, CancellationToken cancellationToken = default);
        Task<UserResult?> GetUserAsync(string userId, CancellationToken cancellationToken = default);
    }
}