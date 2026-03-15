using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class StudentsCoursesEntityTypeConfiguration : IEntityTypeConfiguration<StudentsCourses>
    {
        public void Configure(EntityTypeBuilder<StudentsCourses> builder)
        {
            builder.ToTable("students_courses");

            builder.HasKey(sc => sc.Id);

            builder.Property(sc => sc.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            builder.Property(sc => sc.StudentId)
                .HasColumnName("student_id")
                .HasColumnType("uuid")
                .IsRequired();

            builder.Property(sc => sc.CourseId)
                .HasColumnName("course_id")
                .HasColumnType("uuid")
                .IsRequired();

            builder.HasIndex(sc => new { sc.StudentId, sc.CourseId })
                .IsUnique();

            builder.Property(sc => sc.CreatedBy)
                .HasColumnName("created_by")
                .HasColumnType("varchar(255)");

            builder.Property(sc => sc.UpdatedBy)
                .HasColumnName("modified_by")
                .HasColumnType("varchar(255)");

            builder.Property(sc => sc.CreatedDate)
                .HasColumnName("created_date")
                .IsRequired();

            builder.Property(sc => sc.UpdatedDate)
                .HasColumnName("modified_date");

            builder.Property(sc => sc.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);
        }
    }
}
