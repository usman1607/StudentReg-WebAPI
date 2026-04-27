using Application.Dtos.Common;
using Domain.Entities;
using Domain.Enums;

namespace Application.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment?> GetByIdAsync(Guid id);
        Task<Payment?> GetByPaymentReferenceAsync(string reference);
        Task<Payment?> GetByPaystackReferenceAsync(string paystackReference);
        Task<List<Payment>> GetByStudentIdAsync(Guid studentId);
        Task<PagedResult<Payment>> GetAllAsync(PaymentStatus? status, PaymentType? type, int page, int pageSize);
        Task<Payment> UpdateAsync(Payment payment);
    }
}
