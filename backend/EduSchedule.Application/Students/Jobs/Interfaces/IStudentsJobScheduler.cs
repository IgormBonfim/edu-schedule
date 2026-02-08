namespace EduSchedule.Application.Students.Jobs.Interfaces;

public interface IStudentJobScheduler
{
    void EnqueueStudentSyncBatch(IEnumerable<string> externalIds);
}