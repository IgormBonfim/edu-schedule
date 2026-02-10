using EduSchedule.Application.Students.Jobs.Interfaces;
using EduSchedule.Domain.Integrations.Models;
using EduSchedule.Infrastructure.Jobs.Processors;
using Hangfire;

namespace EduSchedule.Infrastructure.Jobs.Schedulers
{
    public class HangfireStudentJobScheduler : IStudentJobScheduler
    {
        private readonly IBackgroundJobClient _client;

        public HangfireStudentJobScheduler(IBackgroundJobClient client)
        {
            _client = client;
        }

        public void EnqueueStudentSyncBatch(IEnumerable<UserResult> users)
        {
            _client.Enqueue<StudentJobProcessor>(job => job.ProcessBatchAsync(users));
        }
    }
}