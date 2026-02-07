using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EduSchedule.Infrastructure.Database
{
    public class EduScheduleDbContext : DbContext
    {
        public EduScheduleDbContext(DbContextOptions<EduScheduleDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
