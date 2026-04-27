namespace Application.Dtos.ResponseDto
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = default!;
        public string PaymentType { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string PaymentReference { get; set; } = default!;
        public string? PaystackReference { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? PayerName { get; set; }
        public string? ConfirmedBy { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
