using EduSchedule.Domain.Integrations.Models;

namespace EduSchedule.Domain.Integrations.Services.Interfaces
{
    public interface IGraphService
    {
        Task<UsersDeltaResult> GetUsersDeltaAsync(string? deltaToken, CancellationToken cancellationToken = default);
    }
}