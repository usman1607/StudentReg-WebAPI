using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class StudentEntityTypeConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            // Student-specific properties only (base properties configured in UserEntityTypeConfiguration)
            builder.Property(s => s.MatricNumber)
                .HasColumnName("matric_number")
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.HasIndex(s => s.MatricNumber)
                .IsUnique();

            builder.Property(s => s.Status)
                .HasColumnName("status")
                .HasColumnType("varchar(50)")
                .HasConversion<EnumToStringConverter<StudentStatus>>()
                .IsRequired();

            // Configure StudentCourses relationship
            builder.HasMany(u => u.StudentsCourses)
                .WithOne(ur => ur.Student)
                .HasForeignKey(ur => ur.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
