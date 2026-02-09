namespace EduSchedule.Application.Students.Services.Interfaces
{
    public interface ISyncStudentsAppService
    {
        Task SyncBatchStudentEventsAsync(IEnumerable<string> externalIds, CancellationToken cancellationToken = default);
        Task StartSyncProcessAsync(CancellationToken cancellationToken = default);
    }
}