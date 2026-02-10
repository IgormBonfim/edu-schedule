using EduSchedule.Domain.Students.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduSchedule.Infrastructure.Students.Mappings
{
    public class StudentMap : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");

            builder
                .HasKey(s => s.Id);

            builder
                .Property(s => s.ExternalId)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property(s => s.DisplayName)
                .HasMaxLength(250)
                .IsRequired();

            builder
                .Property(s => s.Email)
                .HasMaxLength(150)
                .IsRequired();

            builder
                .Property(s => s.IsActive)
                .IsRequired();
            
            builder
                .Property(s => s.CreatedAt)
                .IsRequired();

            builder
                .Property(s => s.UpdatedAt);

            builder
                .Property(s => s.EventsDeltaToken);

            builder
                .HasIndex(s => s.ExternalId)
                .IsUnique();

            builder
                .HasIndex(s => s.Email)
                .IsUnique();
        }
    }
}