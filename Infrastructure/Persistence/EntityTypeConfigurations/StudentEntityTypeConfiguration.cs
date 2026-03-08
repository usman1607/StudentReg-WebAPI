using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class StudentEntityTypeConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("students");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName)
                .HasColumnName("first_name")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(u => u.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

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

            builder.HasIndex(s => s.MatricNumber)
                .IsUnique();

            builder.Property(s => s.Address)
                .HasColumnName("address")
                .HasColumnType("varchar(255)");

            builder.Property(s => s.PhoneNumber)
                .HasColumnName("phone_number")
                .HasColumnType("varchar(50)");

            builder.Property(s => s.MatricNumber)
                .HasColumnName("matric_number")
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasColumnType("varchar(255)")
                .IsRequired();

            builder.Property(u => u.HashSalt)
                .HasColumnName("hash_salt")
                .HasColumnType("varchar(255)")
                .IsRequired();

            builder.Property(u => u.UserType)
            .HasColumnName("user_type")
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


        }
    }
}
