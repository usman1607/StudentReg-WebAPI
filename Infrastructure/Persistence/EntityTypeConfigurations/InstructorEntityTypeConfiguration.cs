using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class InstructorEntityTypeConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.Property(s => s.StaffNumber)
                .HasColumnName("matric_number")
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.HasIndex(s => s.StaffNumber)
                .IsUnique();
        }
    }
}
