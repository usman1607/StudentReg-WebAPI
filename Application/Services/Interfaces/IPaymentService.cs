using Application.Dtos.Common;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Domain.Enums;

namespace Application.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaystackInitDto> InitiatePaystackPaymentAsync(Guid studentId, string studentEmail, InitiatePaystackPaymentRequest request);
        Task HandlePaystackWebhookAsync(string rawBody, string signature);
        Task<PaymentDto> SubmitManualPaymentAsync(Guid studentId, ManualPaymentRequest request);
        Task<PaymentDto> ConfirmManualPaymentAsync(Guid paymentId, string confirmedBy);
        Task<PaymentDto> RejectPaymentAsync(Guid paymentId, string rejectedBy, string reason);
        Task<PaymentDto?> GetPaymentByIdAsync(Guid paymentId);
        Task<List<PaymentDto>> GetStudentPaymentsAsync(Guid studentId);
        Task<PagedResult<PaymentDto>> GetAllPaymentsAsync(PaymentStatus? status, PaymentType? type, int page, int pageSize);
        BankAccountDto GetBankAccountDetails();
    }
}
