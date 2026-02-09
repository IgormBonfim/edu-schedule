using EduSchedule.Domain.States.Entities;
using EduSchedule.Domain.States.Repositories;

namespace EduSchedule.Infrastructure.Database.Repositories
{
    public class SyncStatesRepository : EFRepository<SyncState>, ISyncStatesRepository
    {
        public SyncStatesRepository(EduScheduleDbContext context) : base(context) { }
    }
}