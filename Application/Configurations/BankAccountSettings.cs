namespace Application.Configurations
{
    public class BankAccountSettings
    {
        public string BankName { get; set; } = default!;
        public string AccountName { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public string? PaymentInstruction { get; set; }
    }
}
