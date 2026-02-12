using EduSchedule.Domain.Integrations.Models;

namespace EduSchedule.Application.Students.Jobs.Interfaces;

public interface IStudentJobScheduler
{
    void EnqueueStudentSyncBatch(IEnumerable<UserResult> users);
    void EnqueueStudentSyncEventsBatch(int skip, int take);
}