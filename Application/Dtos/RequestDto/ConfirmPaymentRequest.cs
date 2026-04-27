namespace Application.Dtos.RequestDto
{
    public class ConfirmPaymentRequest
    {
        public string? Note { get; set; }
    }

    public class RejectPaymentRequest
    {
        public string Reason { get; set; } = default!;
    }
}
