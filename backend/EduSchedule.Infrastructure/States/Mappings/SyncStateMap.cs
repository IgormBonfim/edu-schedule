using EduSchedule.Domain.States.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduSchedule.Infrastructure.States.Mappings
{
    public class SyncStateMap : IEntityTypeConfiguration<SyncState>
    {
        public void Configure(EntityTypeBuilder<SyncState> builder)
        {
            builder.ToTable("SyncStates");
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.EntityName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.DeltaToken).IsRequired();
        }
    }
}