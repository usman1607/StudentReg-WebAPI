using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        }
    }
}
