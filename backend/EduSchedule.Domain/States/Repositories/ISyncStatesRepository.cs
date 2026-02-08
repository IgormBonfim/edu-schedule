using EduSchedule.Domain.Repositories;
using EduSchedule.Domain.States.Entities;

namespace EduSchedule.Domain.States.Repositories
{
    public interface ISyncStatesRepository : IRepository<SyncState> { }
}