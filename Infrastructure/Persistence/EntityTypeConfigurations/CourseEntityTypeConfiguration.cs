using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    internal class CourseEntityTypeConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("courses");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(c => c.Code)
                .HasColumnName("code")
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.HasIndex(c => c.Code)
                .IsUnique();

            builder.Property(c => c.Description)
                .HasColumnName("description")
                .HasColumnType("varchar(255)");
        }
    }
}
