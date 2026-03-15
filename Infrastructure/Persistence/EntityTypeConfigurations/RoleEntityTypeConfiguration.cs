using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            builder.Property(r => r.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.HasIndex(r => r.Name)
                .IsUnique();

            builder.Property(r => r.Description)
                .HasColumnName("description")
                .HasColumnType("varchar(255)");

            builder.Property(r => r.CreatedBy)
                .HasColumnName("created_by")
                .HasColumnType("varchar(255)");

            builder.Property(r => r.UpdatedBy)
                .HasColumnName("modified_by")
                .HasColumnType("varchar(255)");

            builder.Property(r => r.CreatedDate)
                .HasColumnName("created_date")
                .IsRequired();

            builder.Property(r => r.UpdatedDate)
                .HasColumnName("modified_date");

            builder.Property(r => r.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);
        }
    }
}
