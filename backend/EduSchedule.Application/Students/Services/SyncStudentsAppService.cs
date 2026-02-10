using EduSchedule.Application.Students.Jobs.Interfaces;
using EduSchedule.Domain.Integrations.Models;
using EduSchedule.Domain.Integrations.Services.Interfaces;
using EduSchedule.Domain.States.Entities;
using EduSchedule.Domain.States.Repositories;
using EduSchedule.Domain.Students.Entities;
using EduSchedule.Domain.Students.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EduSchedule.Application.Students.Services.Interfaces
{
    public class SyncStudentsAppService : ISyncStudentsAppService
    {
        private readonly IStudentJobScheduler _studentJobScheduler;
        private readonly ISyncStatesRepository _syncStatesRepository;
        private readonly IGraphService _graphService;
        private readonly ILogger<SyncStudentsAppService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private const string STUDENT_ENTITY_NAME = "Students";

        public SyncStudentsAppService(IStudentJobScheduler studentJobScheduler, ISyncStatesRepository syncStatesRepository, IGraphService graphService, ILogger<SyncStudentsAppService> logger, IServiceScopeFactory scopeFactory)
        {
            _studentJobScheduler = studentJobScheduler;
            _syncStatesRepository = syncStatesRepository;
            _graphService = graphService;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task StartStudensSyncProcessAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Iniciando sincronismo de estudantes...");

            try
            {
                SyncState? state = await _syncStatesRepository.GetAsync(x => x.EntityName == STUDENT_ENTITY_NAME, cancellationToken);
            
                string? currentDeltaToken = state?.DeltaToken;
                string? currentNextLink = null;
                string finalDeltaToken = string.Empty;

                do
                {
                    var result = await _graphService.GetUsersDeltaAsync(currentDeltaToken, currentNextLink, cancellationToken: cancellationToken);

                    if (result.ChangedUsers.Any())
                    {
                        var chunks = result.ChangedUsers.Chunk(100); 

                        foreach (var chunk in chunks)
                        {
                            _studentJobScheduler.EnqueueStudentSyncBatch(chunk.ToList());
                            _logger.LogInformation("Página processada: {Count} IDs enviados para processamento.", result.ChangedUsers.Count());
                        }
                    }

                    if (!string.IsNullOrEmpty(result.NextDeltaToken))
                    {
                        finalDeltaToken = result.NextDeltaToken;
                    }

                    currentNextLink = result.NextDeltaLink;
                    
                    currentDeltaToken = null; 

                } while (!string.IsNullOrEmpty(currentNextLink));

                _logger.LogInformation("Sincronismo finalizado. Novo DeltaToken capturado.");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao sincronizar estudantes.");
                throw;
            }
        }

        public async Task SyncBatchStudentsAsync(IEnumerable<UserResult> users, CancellationToken cancellationToken = default)
        {
            foreach (UserResult user in users)
            {
                await ProcessUserAsync(user, cancellationToken);
            }
        }

        public async Task ProcessUserAsync(UserResult user, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<IStudentsRepository>();

                    Student? student = await repo.GetAsync(x => x.ExternalId == user.Id);

                    if (student is null)
                    {
                        if (user.IsDeleted) return;

                        student = new Student(user.Id, user.DisplayName, user.Email);
                        student = await repo.InsertAsync(student);
                    }
                    else
                    {
                        if (user.IsDeleted)
                        {
                            student.Inactivate();
                        }
                        else
                        {
                            student.Update(user.DisplayName, user.Email);
                        }
                        await repo.UpdateAsync(student, cancellationToken);
                    }

                    _logger.LogInformation("Usuário processado com sucesso. IdExterno: {idExterno}", user.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao processar o Estudante. IdExterno: {idExterno}", user.Id);
            }
        }

        public Task StartEventsSyncProcessAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}