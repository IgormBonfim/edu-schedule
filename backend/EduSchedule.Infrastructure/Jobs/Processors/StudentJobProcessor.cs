using EduSchedule.Application.Students.Services.Interfaces;
using EduSchedule.Domain.Integrations.Models;
using Hangfire;

namespace EduSchedule.Infrastructure.Jobs.Processors
{
    public class StudentJobProcessor
    {
        private readonly ISyncStudentsAppService _syncStudentsAppService;
        public StudentJobProcessor(ISyncStudentsAppService syncStudentsAppService)
        {
            _syncStudentsAppService = syncStudentsAppService;
        }

        [Queue("student-sync")]
        public void ProcessBatchAsync(IEnumerable<UserResult> users)
        {
            _syncStudentsAppService.SyncBatchStudentsAsync(users);
        }
    }
    
}