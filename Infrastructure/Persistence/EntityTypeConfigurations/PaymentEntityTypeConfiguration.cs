using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.StudentId).HasColumnName("student_id").IsRequired();
            builder.Property(p => p.Amount).HasColumnName("amount").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.Description).HasColumnName("description").HasMaxLength(500).IsRequired();

            builder.Property(p => p.PaymentType)
                .HasColumnName("payment_type")
                .HasColumnType("varchar(50)")
                .HasConversion<EnumToStringConverter<PaymentType>>()
                .IsRequired();

            builder.Property(p => p.Status)
                .HasColumnName("status")
                .HasColumnType("varchar(50)")
                .HasConversion<EnumToStringConverter<PaymentStatus>>()
                .IsRequired();

            builder.Property(p => p.PaymentReference).HasColumnName("payment_reference").HasMaxLength(100).IsRequired();
            builder.HasIndex(p => p.PaymentReference).IsUnique();

            builder.Property(p => p.PaystackReference).HasColumnName("paystack_reference").HasMaxLength(200);
            builder.Property(p => p.PaystackAuthorizationUrl).HasColumnName("paystack_authorization_url").HasMaxLength(500);
            builder.Property(p => p.PaystackAccessCode).HasColumnName("paystack_access_code").HasMaxLength(200);

            builder.Property(p => p.BankName).HasColumnName("bank_name").HasMaxLength(200);
            builder.Property(p => p.AccountName).HasColumnName("account_name").HasMaxLength(200);
            builder.Property(p => p.AccountNumber).HasColumnName("account_number").HasMaxLength(50);
            builder.Property(p => p.PayerName).HasColumnName("payer_name").HasMaxLength(200);

            builder.Property(p => p.ConfirmedBy).HasColumnName("confirmed_by").HasMaxLength(200);
            builder.Property(p => p.ConfirmedAt).HasColumnName("confirmed_at");
            builder.Property(p => p.RejectionReason).HasColumnName("rejection_reason").HasMaxLength(1000);

            builder.Property(p => p.CreatedBy).HasColumnName("created_by");
            builder.Property(p => p.UpdatedBy).HasColumnName("updated_by");
            builder.Property(p => p.CreatedDate).HasColumnName("created_date");
            builder.Property(p => p.UpdatedDate).HasColumnName("updated_date");
            builder.Property(p => p.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

            builder.HasOne(p => p.Student)
                .WithMany()
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
