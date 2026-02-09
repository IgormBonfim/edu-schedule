namespace EduSchedule.Application.Students.Services.Interfaces
{
    public interface ISyncStudentsAppService
    {
        Task SyncBatchStudentsAsync(IEnumerable<string> externalIds, CancellationToken cancellationToken = default);
        Task StartStudensSyncProcessAsync(CancellationToken cancellationToken = default);
        Task StartEventsSyncProcessAsync(CancellationToken cancellationToken = default);
    }
}