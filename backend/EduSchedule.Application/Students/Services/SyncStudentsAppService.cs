using System.Data.Common;
using EduSchedule.Application.Students.Jobs.Interfaces;
using EduSchedule.Domain.States.Entities;
using EduSchedule.Domain.States.Repositories;

namespace EduSchedule.Application.Students.Services.Interfaces
{
    public class SyncStudentsAppService : ISyncStudentsAppService
    {
        private readonly IStudentJobScheduler _studentJobScheduler;
        private readonly ISyncStatesRepository _syncStatesRepository;
        private const string STUDENT_ENTITY_NAME = "Students";

        public SyncStudentsAppService(IStudentJobScheduler studentJobScheduler, ISyncStatesRepository syncStatesRepository)
        {
            _studentJobScheduler = studentJobScheduler;
            _syncStatesRepository = syncStatesRepository;
        }

        public async Task StartSyncProcessAsync()
        {
            SyncState? state = await _syncStatesRepository.GetAsync(x => x.EntityName == STUDENT_ENTITY_NAME);
            // IEnumerable<string> allStudentExternalIds = await _graphService.GetStudentIdsForSyncAsync();

            IEnumerable<string> ids = new List<string>() { "Eu", "Amo", "A", "Sissa" };

            _studentJobScheduler.EnqueueStudentSyncBatch(ids);
        }

        public async Task SyncBatchStudentEventsAsync(IEnumerable<string> externalIds, CancellationToken cancellationToken = default)
        {
            foreach (string id in externalIds)
            {
                Console.WriteLine(id);
            }
        }
    }
}