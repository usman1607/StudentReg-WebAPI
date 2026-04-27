using Application.Configurations;
using Application.Dtos.Common;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Exceptions;
using Application.Repositories;
using Application.Services.Contracts;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Application.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaystackService _paystackService;
        private readonly BankAccountSettings _bankAccountSettings;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IPaystackService paystackService,
            IOptions<BankAccountSettings> bankAccountSettings,
            ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _paystackService = paystackService;
            _bankAccountSettings = bankAccountSettings.Value;
            _logger = logger;
        }

        public async Task<PaystackInitDto> InitiatePaystackPaymentAsync(
            Guid studentId, string studentEmail, InitiatePaystackPaymentRequest request)
        {
            var reference = GenerateReference();

            _logger.LogInformation("Initiating Paystack payment for student {StudentId}, ref: {Ref}", studentId, reference);

            var result = await _paystackService.InitializeTransactionAsync(
                studentEmail, request.Amount, reference, request.CallbackUrl);

            var payment = new Payment
            {
                StudentId = studentId,
                Amount = request.Amount,
                Description = request.Description,
                PaymentType = PaymentType.Paystack,
                Status = PaymentStatus.Pending,
                PaymentReference = reference,
                PaystackReference = result.Reference,
                PaystackAuthorizationUrl = result.AuthorizationUrl,
                PaystackAccessCode = result.AccessCode,
                CreatedBy = studentEmail
            };

            var created = await _paymentRepository.CreateAsync(payment);

            _logger.LogInformation("Paystack payment record created: {PaymentId}", created.Id);

            return new PaystackInitDto
            {
                PaymentId = created.Id,
                PaymentReference = created.PaymentReference,
                AuthorizationUrl = result.AuthorizationUrl,
                AccessCode = result.AccessCode
            };
        }

        public async Task HandlePaystackWebhookAsync(string rawBody, string signature)
        {
            if (!_paystackService.VerifyWebhookSignature(rawBody, signature))
            {
                _logger.LogWarning("Invalid Paystack webhook signature received");
                throw new UnauthorizedAccessException("Invalid webhook signature.");
            }

            using var doc = JsonDocument.Parse(rawBody);
            var root = doc.RootElement;

            var eventType = root.TryGetProperty("event", out var ev) ? ev.GetString() : null;
            if (eventType != "charge.success")
            {
                _logger.LogInformation("Paystack webhook event {Event} — no action taken", eventType);
                return;
            }

            var data = root.GetProperty("data");
            var paystackReference = data.GetProperty("reference").GetString()!;
            var amountKobo = data.GetProperty("amount").GetInt64();
            var status = data.GetProperty("status").GetString() ?? "";

            _logger.LogInformation("Processing charge.success webhook for ref: {Ref}", paystackReference);

            var payment = await _paymentRepository.GetByPaystackReferenceAsync(paystackReference);
            if (payment == null)
            {
                _logger.LogWarning("No payment found for Paystack reference: {Ref}", paystackReference);
                return;
            }

            if (payment.Status == PaymentStatus.Confirmed)
            {
                _logger.LogInformation("Payment {PaymentId} already confirmed — skipping", payment.Id);
                return;
            }

            payment.Status = status == "success" ? PaymentStatus.Confirmed : PaymentStatus.Failed;
            payment.ConfirmedBy = "Paystack";
            payment.ConfirmedAt = DateTime.UtcNow;
            payment.UpdatedDate = DateTime.UtcNow;
            payment.UpdatedBy = "Paystack";

            await _paymentRepository.UpdateAsync(payment);

            _logger.LogInformation("Payment {PaymentId} updated to {Status} via webhook", payment.Id, payment.Status);
        }

        public async Task<PaymentDto> SubmitManualPaymentAsync(Guid studentId, ManualPaymentRequest request)
        {
            _logger.LogInformation("Student {StudentId} submitting manual payment, ref: {Ref}",
                studentId, request.PaymentReference);

            var existing = await _paymentRepository.GetByPaymentReferenceAsync(request.PaymentReference);
            if (existing != null)
            {
                throw new ValidationException($"A payment with reference '{request.PaymentReference}' already exists.");
            }

            var payment = new Payment
            {
                StudentId = studentId,
                Amount = request.Amount,
                Description = request.Description,
                PaymentType = PaymentType.Manual,
                Status = PaymentStatus.Pending,
                PaymentReference = request.PaymentReference,
                BankName = request.BankName,
                AccountName = request.AccountName,
                AccountNumber = request.AccountNumber,
                PayerName = request.PayerName,
                CreatedBy = studentId.ToString()
            };

            var created = await _paymentRepository.CreateAsync(payment);
            _logger.LogInformation("Manual payment record created: {PaymentId}", created.Id);
            return MapToDto(created);
        }

        public async Task<PaymentDto> ConfirmManualPaymentAsync(Guid paymentId, string confirmedBy)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId)
                ?? throw new EntityNotFoundException($"Payment with ID '{paymentId}' not found.");

            if (payment.PaymentType != PaymentType.Manual)
                throw new ValidationException("Only manual payments can be confirmed this way.");

            if (payment.Status != PaymentStatus.Pending)
                throw new ValidationException($"Payment is not in Pending state. Current status: {payment.Status}");

            payment.Status = PaymentStatus.Confirmed;
            payment.ConfirmedBy = confirmedBy;
            payment.ConfirmedAt = DateTime.UtcNow;
            payment.UpdatedDate = DateTime.UtcNow;
            payment.UpdatedBy = confirmedBy;

            var updated = await _paymentRepository.UpdateAsync(payment);
            _logger.LogInformation("Manual payment {PaymentId} confirmed by {ConfirmedBy}", paymentId, confirmedBy);
            return MapToDto(updated);
        }

        public async Task<PaymentDto> RejectPaymentAsync(Guid paymentId, string rejectedBy, string reason)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId)
                ?? throw new EntityNotFoundException($"Payment with ID '{paymentId}' not found.");

            if (payment.Status != PaymentStatus.Pending)
                throw new ValidationException($"Payment is not in Pending state. Current status: {payment.Status}");

            payment.Status = PaymentStatus.Rejected;
            payment.RejectionReason = reason;
            payment.UpdatedDate = DateTime.UtcNow;
            payment.UpdatedBy = rejectedBy;

            var updated = await _paymentRepository.UpdateAsync(payment);
            _logger.LogInformation("Payment {PaymentId} rejected by {RejectedBy}. Reason: {Reason}", paymentId, rejectedBy, reason);
            return MapToDto(updated);
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(Guid paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            return payment == null ? null : MapToDto(payment);
        }

        public async Task<List<PaymentDto>> GetStudentPaymentsAsync(Guid studentId)
        {
            var payments = await _paymentRepository.GetByStudentIdAsync(studentId);
            return payments.Select(MapToDto).ToList();
        }

        public async Task<PagedResult<PaymentDto>> GetAllPaymentsAsync(
            PaymentStatus? status, PaymentType? type, int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var paged = await _paymentRepository.GetAllAsync(status, type, page, pageSize);
            var dtos = paged.Items.Select(MapToDto).ToList();
            return new PagedResult<PaymentDto>(dtos, page, pageSize, paged.TotalCount);
        }

        public BankAccountDto GetBankAccountDetails()
        {
            return new BankAccountDto
            {
                BankName = _bankAccountSettings.BankName,
                AccountName = _bankAccountSettings.AccountName,
                AccountNumber = _bankAccountSettings.AccountNumber,
                PaymentInstruction = _bankAccountSettings.PaymentInstruction
            };
        }

        private static string GenerateReference()
        {
            return $"STU-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }

        private static PaymentDto MapToDto(Payment payment) => new()
        {
            Id = payment.Id,
            StudentId = payment.StudentId,
            Amount = payment.Amount,
            Description = payment.Description,
            PaymentType = payment.PaymentType.ToString(),
            Status = payment.Status.ToString(),
            PaymentReference = payment.PaymentReference,
            PaystackReference = payment.PaystackReference,
            BankName = payment.BankName,
            AccountName = payment.AccountName,
            AccountNumber = payment.AccountNumber,
            PayerName = payment.PayerName,
            ConfirmedBy = payment.ConfirmedBy,
            ConfirmedAt = payment.ConfirmedAt,
            RejectionReason = payment.RejectionReason,
            CreatedDate = payment.CreatedDate,
            UpdatedDate = payment.UpdatedDate
        };
    }
}
