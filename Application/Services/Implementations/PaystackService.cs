using Application.Configurations;
using Application.Services.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Application.Services.Implementations
{
    public class PaystackService : IPaystackService
    {
        private readonly HttpClient _httpClient;
        private readonly PaystackSettings _settings;
        private readonly ILogger<PaystackService> _logger;

        public PaystackService(HttpClient httpClient, IOptions<PaystackSettings> settings, ILogger<PaystackService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl.TrimEnd('/') + "/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.SecretKey);
        }

        public async Task<PaystackTransactionInitResult> InitializeTransactionAsync(
            string email, decimal amountNaira, string reference, string callbackUrl)
        {
            var body = new
            {
                email,
                amount = (long)(amountNaira * 100), // Paystack expects kobo
                reference,
                callback_url = callbackUrl
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Initializing Paystack transaction for {Email}, ref: {Ref}", email, reference);

            var response = await _httpClient.PostAsync("transaction/initialize", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Paystack initialize failed: {StatusCode} {Body}", response.StatusCode, responseBody);
                throw new InvalidOperationException($"Paystack initialization failed: {responseBody}");
            }

            using var doc = JsonDocument.Parse(responseBody);
            var data = doc.RootElement.GetProperty("data");

            return new PaystackTransactionInitResult(
                AuthorizationUrl: data.GetProperty("authorization_url").GetString()!,
                AccessCode: data.GetProperty("access_code").GetString()!,
                Reference: data.GetProperty("reference").GetString()!
            );
        }

        public async Task<PaystackVerifyResult> VerifyTransactionAsync(string reference)
        {
            _logger.LogInformation("Verifying Paystack transaction: {Ref}", reference);

            var response = await _httpClient.GetAsync($"transaction/verify/{reference}");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Paystack verify failed: {StatusCode} {Body}", response.StatusCode, responseBody);
                return new PaystackVerifyResult(false, "failed", 0, reference, "Verification request failed");
            }

            using var doc = JsonDocument.Parse(responseBody);
            var data = doc.RootElement.GetProperty("data");
            var status = data.GetProperty("status").GetString() ?? "failed";
            var amountKobo = data.GetProperty("amount").GetInt64();
            var gatewayResponse = data.TryGetProperty("gateway_response", out var gr)
                ? gr.GetString() ?? ""
                : "";

            return new PaystackVerifyResult(
                Success: status == "success",
                Status: status,
                Amount: amountKobo / 100m,
                Reference: reference,
                GatewayResponse: gatewayResponse
            );
        }

        public bool VerifyWebhookSignature(string rawBody, string signature)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawBody));
            var computed = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return computed == signature;
        }
    }
}
