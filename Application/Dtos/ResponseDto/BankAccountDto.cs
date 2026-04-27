namespace Application.Dtos.ResponseDto
{
    public class BankAccountDto
    {
        public string BankName { get; set; } = default!;
        public string AccountName { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public string? PaymentInstruction { get; set; }
    }
}
