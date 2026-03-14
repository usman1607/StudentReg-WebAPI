using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class StaffEntityTypeConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.Property(s => s.StaffNumber)
                .HasColumnName("staff_number")
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.HasIndex(s => s.StaffNumber)
                .IsUnique();

            builder.Property(s => s.Delegation)
                .HasColumnName("delegation")
                .HasColumnType("varchar(50)")
                .HasConversion<EnumToStringConverter<StaffDelegation>>()
                .IsRequired();
        }
    }
}
