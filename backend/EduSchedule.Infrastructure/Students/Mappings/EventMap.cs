using EduSchedule.Domain.Students.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduSchedule.Infrastructure.Students.Mappings
{
    public class EventMap : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");

            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.ExternalId)
                .IsRequired()
                .HasMaxLength(150);

            builder
                .Property(x => x.Subject)
                .IsRequired()
                .HasMaxLength(250);

            builder
                .Property(s => s.IsActive)
                .IsRequired();
            
            builder
                .Property(s => s.CreatedAt)
                .IsRequired();

            builder
                .Property(s => s.UpdatedAt);

            builder
                .HasIndex(x => x.ExternalId)
                .IsUnique();

            builder
                .HasOne(x => x.Student)
                .WithMany(s => s.Events)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
