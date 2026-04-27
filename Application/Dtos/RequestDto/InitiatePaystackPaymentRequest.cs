namespace Application.Dtos.RequestDto
{
    public class InitiatePaystackPaymentRequest
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = default!;
        public string CallbackUrl { get; set; } = default!;
    }
}
