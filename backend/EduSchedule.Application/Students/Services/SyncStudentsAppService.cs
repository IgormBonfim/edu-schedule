using System.Collections.Frozen;
using EduSchedule.Application.Students.Jobs.Interfaces;
using EduSchedule.Domain.Integrations.Models;
using EduSchedule.Domain.Integrations.Services.Interfaces;
using EduSchedule.Domain.Repositories;
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
            
                string? deltaToken = null;
                string? nextDeltaLink = state?.NextLink;
                int totalUsersInserted = 0;
                const int CHECKPOINT_THRESHOLD = 5000;

                do
                {
                    var result = await _graphService.GetUsersDeltaAsync(nextDeltaLink, cancellationToken: cancellationToken);

                    if (result.ChangedValues.Any())
                    {
                        var chunks = result.ChangedValues.Chunk(100).ToList();

                        foreach (var chunk in result.ChangedValues.Chunk(100))                    
                        {
                            var usersList = chunk.ToList();
                            _studentJobScheduler.EnqueueStudentSyncBatch(usersList);

                            if (totalUsersInserted >= CHECKPOINT_THRESHOLD && !string.IsNullOrEmpty(result.NextDeltaLink))
                            {
                                totalUsersInserted = 0;
                                state = await SaveSyncStateAsync(state, result.NextDeltaLink, cancellationToken);
                                await _syncStatesRepository.UpdateAsync(state);
                            }

                            totalUsersInserted += usersList.Count;
                        }
                    }

                    nextDeltaLink = result.NextDeltaLink;
                    deltaToken = result.NextDeltaToken; 

                } while (string.IsNullOrEmpty(deltaToken));

                _logger.LogInformation("Sincronismo finalizado. Novo DeltaToken capturado.");
                if (!string.IsNullOrEmpty(nextDeltaLink))
                {
                    await SaveSyncStateAsync(state, nextDeltaLink, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao sincronizar estudantes.");
                throw;
            }
        }

        private async Task<SyncState> SaveSyncStateAsync(SyncState? state, string nextDeltaLink, CancellationToken cancellationToken = default)
        {
            if (state != null)
            {
                state.UpdateNextLink(nextDeltaLink);
                await _syncStatesRepository.UpdateAsync(state, cancellationToken);
            }
            else
            {
                state = new SyncState(STUDENT_ENTITY_NAME, nextDeltaLink);
                await _syncStatesRepository.InsertAsync(state, cancellationToken);
            }

            return state;
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

        public async Task StartEventsSyncProcessAsync(CancellationToken cancellationToken = default)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IStudentsRepository>();
                int batchSize = 1000;
                int skip = 0;
                bool hasMore = true;

                while (hasMore)
                {
                    var students = await repo.GetStudentsBatchAsync(skip, batchSize, cancellationToken);

                    if (students.Any())
                    {
                        _studentJobScheduler.EnqueueStudentSyncEventsBatch(skip, batchSize);

                        skip += batchSize;
                        _logger.LogInformation("StartEventsSyncProcessAsync: {Skip} usuários enfileirados...", skip);
                    }
                    else
                    {
                        hasMore = false;
                    }
                }
            }
        }

        public async Task SyncBatchStudentsEventsAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IStudentsRepository>();
                var students = await repo.GetStudentsBatchAsync(skip, take, cancellationToken);

                foreach (var student in students)
                {
                    try
                    {
                        var updatedStudent = await ProcessStudentEventsAsync(student, cancellationToken);
                        await repo.UpdateAsync(updatedStudent);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao sincronizar eventos do aluno {ExternalId}", student.ExternalId);                
                    } 
                }
            }
        }

        private async Task<Student> ProcessStudentEventsAsync(Student student, CancellationToken cancellationToken = default)
        {
            string? currentToken = student.EventsDeltaToken;
            DeltaResults<EventResult> results;

            do
            {
                results = await _graphService.GetUserEventsDeltaAsync(student.ExternalId, currentToken, cancellationToken);
                _logger.LogInformation("Aluno {Id} retornou {Count} eventos do Graph.", student.ExternalId, results.ChangedValues.Count());
                if (results.ChangedValues.Any())
                {
                    foreach (var eventResult in results.ChangedValues)
                    {
                        Event? evento = student.Events.FirstOrDefault(x => x.ExternalId == eventResult.Id);

                        var start = eventResult.Start?.UtcDateTime ?? DateTime.MinValue;
                        var end = eventResult.End?.UtcDateTime ?? DateTime.MinValue;

                        if (evento is null)
                        {
                            evento = new Event(eventResult.Id, eventResult.Subject, start, end, student.Id);
                            student.Events.Add(evento);
                        }
                        else
                        {
                            evento.Update(eventResult.Subject, start, end);
                        }
                    }
                }

                currentToken = results.NextDeltaLink;

            } while (string.IsNullOrEmpty(results.NextDeltaToken));

            student.UpdateEventsDeltaToken(currentToken);

            return student;
        }
    }
}