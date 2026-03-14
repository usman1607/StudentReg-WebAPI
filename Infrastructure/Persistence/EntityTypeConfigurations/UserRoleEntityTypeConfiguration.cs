using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("user_roles");

            builder.HasKey(ur => ur.Id);

            builder.Property(ur => ur.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            builder.Property(ur => ur.UserId)
                .HasColumnName("user_id")
                .HasColumnType("uuid")
                .IsRequired();

            builder.Property(ur => ur.RoleId)
                .HasColumnName("role_id")
                .HasColumnType("uuid")
                .IsRequired();

            builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            builder.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ur => ur.CreatedBy)
                .HasColumnName("created_by")
                .HasColumnType("varchar(255)");

            builder.Property(ur => ur.UpdatedBy)
                .HasColumnName("modified_by")
                .HasColumnType("varchar(255)");

            builder.Property(ur => ur.CreatedDate)
                .HasColumnName("created_date")
                .IsRequired();

            builder.Property(ur => ur.UpdatedDate)
                .HasColumnName("modified_date");

            builder.Property(ur => ur.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);
        }
    }
}
