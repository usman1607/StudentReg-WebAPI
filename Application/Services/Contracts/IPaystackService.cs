namespace Application.Services.Contracts
{
    public interface IPaystackService
    {
        Task<PaystackTransactionInitResult> InitializeTransactionAsync(string email, decimal amountNaira, string reference, string callbackUrl);
        Task<PaystackVerifyResult> VerifyTransactionAsync(string reference);
        bool VerifyWebhookSignature(string rawBody, string signature);
    }

    public record PaystackTransactionInitResult(string AuthorizationUrl, string AccessCode, string Reference);

    public record PaystackVerifyResult(bool Success, string Status, decimal Amount, string Reference, string GatewayResponse);
}
