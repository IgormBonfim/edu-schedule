using EduSchedule.Infrastructure.Database.Models;
using EduSchedule.Infrastructure.Database.Repositories.Interfaces;

namespace EduSchedule.Infrastructure.Database.Repositories
{
    public class SyncStatesRepository : Repository<SyncState>, ISyncStatesRepository
    {
        public SyncStatesRepository(EduScheduleDbContext context) : base(context) { }
    }
}