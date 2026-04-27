namespace Application.Dtos.ResponseDto
{
    public class PaystackInitDto
    {
        public Guid PaymentId { get; set; }
        public string PaymentReference { get; set; } = default!;
        public string AuthorizationUrl { get; set; } = default!;
        public string AccessCode { get; set; } = default!;
    }
}
