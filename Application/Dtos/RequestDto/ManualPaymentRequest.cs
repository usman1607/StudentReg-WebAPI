namespace Application.Dtos.RequestDto
{
    public class ManualPaymentRequest
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = default!;
        public string PaymentReference { get; set; } = default!;
        public string BankName { get; set; } = default!;
        public string AccountName { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public string PayerName { get; set; } = default!;
    }
}
