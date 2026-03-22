using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Configure TPH (Table-Per-Hierarchy) inheritance
            builder.ToTable("users");

            // Key must be on the root/base type
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            builder.Property(u => u.FirstName)
                .HasColumnName("first_name")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasColumnName("last_name")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(u => u.Email)
                .HasColumnName("email")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.ProfilePictureUrl)
                .HasColumnName("profile_picture_url");

            builder.Property(u => u.Address)
                .HasColumnName("address")
                .HasColumnType("varchar(255)");

            builder.Property(u => u.PhoneNumber)
                .HasColumnName("phone_number")
                .HasColumnType("varchar(50)");

            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasColumnType("varchar(255)")
                .IsRequired();

            builder.Property(u => u.HashSalt)
                .HasColumnName("hash_salt")
                .HasColumnType("varchar(255)")
                .IsRequired();

            builder.Property(u => u.UserType)
                .HasColumnName("user_type_enum")
                .HasColumnType("varchar(50)")
                .HasConversion<EnumToStringConverter<UserType>>()
                .IsRequired();

            builder.Property(u => u.Gender)
                .HasColumnName("gender")
                .HasColumnType("varchar(50)")
                .HasConversion<EnumToStringConverter<Gender>>()
                .IsRequired();

            builder.Property(u => u.CreatedBy)
                .HasColumnName("created_by")
                .HasColumnType("varchar(255)");

            builder.Property(u => u.UpdatedBy)
                .HasColumnName("modified_by")
                .HasColumnType("varchar(255)");

            builder.Property(u => u.CreatedDate)
                .HasColumnName("created_date")
                .IsRequired();

            builder.Property(u => u.UpdatedDate)
                .HasColumnName("modified_date");

            builder.Property(u => u.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);

            // Discriminator for TPH
            builder.HasDiscriminator<string>("user_type")
                .HasValue<Student>("Student")
                .HasValue<Staff>("Staff");

            // Configure UserRoles relationship
            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
