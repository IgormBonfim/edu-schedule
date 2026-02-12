using EduSchedule.Domain.Integrations.Models;

namespace EduSchedule.Application.Students.Services.Interfaces
{
    public interface ISyncStudentsAppService
    {
        Task SyncBatchStudentsAsync(IEnumerable<UserResult> users, CancellationToken cancellationToken = default);
        Task StartStudensSyncProcessAsync(CancellationToken cancellationToken = default);
        Task StartEventsSyncProcessAsync(CancellationToken cancellationToken = default);
        Task SyncBatchStudentsEventsAsync(int skip, int take, CancellationToken cancellationToken = default);
    }
}