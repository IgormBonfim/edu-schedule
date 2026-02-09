using System.Data.Common;
using EduSchedule.Application.Students.Jobs.Interfaces;
using EduSchedule.Domain.Integrations.Models;
using EduSchedule.Domain.Integrations.Services.Interfaces;
using EduSchedule.Domain.States.Entities;
using EduSchedule.Domain.States.Repositories;

namespace EduSchedule.Application.Students.Services.Interfaces
{
    public class SyncStudentsAppService : ISyncStudentsAppService
    {
        private readonly IStudentJobScheduler _studentJobScheduler;
        private readonly ISyncStatesRepository _syncStatesRepository;
        private readonly IGraphService _graphService;
        private const string STUDENT_ENTITY_NAME = "Students";

        public SyncStudentsAppService(IStudentJobScheduler studentJobScheduler, ISyncStatesRepository syncStatesRepository, IGraphService graphService)
        {
            _studentJobScheduler = studentJobScheduler;
            _syncStatesRepository = syncStatesRepository;
            _graphService = graphService;
        }

        public async Task StartStudensSyncProcessAsync(CancellationToken cancellationToken = default)
        {
            SyncState? state = await _syncStatesRepository.GetAsync(x => x.EntityName == STUDENT_ENTITY_NAME, cancellationToken);
            
            string? currentDeltaToken = state?.DeltaToken;
            string? currentNextLink = null;
            string finalDeltaToken = string.Empty;

            do
            {
                var result = await _graphService.GetUsersDeltaAsync(currentDeltaToken, currentNextLink, cancellationToken: cancellationToken);

                if (result.ChangedIds.Any())
                {
                    var chunks = result.ChangedIds.Chunk(100); 

                    foreach (var chunk in chunks)
                    {
                        _studentJobScheduler.EnqueueStudentSyncBatch(chunk.ToList());
                    }
                }

                if (!string.IsNullOrEmpty(result.NextDeltaToken))
                {
                    finalDeltaToken = result.NextDeltaToken;
                }

                currentNextLink = result.NextDeltaLink;
                
                currentDeltaToken = null; 

            } while (!string.IsNullOrEmpty(currentNextLink));

            if (!string.IsNullOrEmpty(finalDeltaToken))
            {
                if (state != null)
                {
                    state.UpdateDeltaToken(finalDeltaToken);
                    await _syncStatesRepository.UpdateAsync(state, cancellationToken);
                }
                else
                {
                    state = new SyncState(STUDENT_ENTITY_NAME, finalDeltaToken);
                    await _syncStatesRepository.InsertAsync(state, cancellationToken);
                }
            }
        }

        public async Task SyncBatchStudentsAsync(IEnumerable<string> externalIds, CancellationToken cancellationToken = default)
        {
            foreach (string id in externalIds)
            {
                Console.WriteLine(id);
            }
        }

        public Task StartEventsSyncProcessAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}