using Application.Dtos.Common;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _context.Payments
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Payment?> GetByPaymentReferenceAsync(string reference)
        {
            return await _context.Payments
                .Where(p => p.PaymentReference == reference && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Payment?> GetByPaystackReferenceAsync(string paystackReference)
        {
            return await _context.Payments
                .Where(p => p.PaystackReference == paystackReference && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Payment>> GetByStudentIdAsync(Guid studentId)
        {
            return await _context.Payments
                .Where(p => p.StudentId == studentId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<PagedResult<Payment>> GetAllAsync(PaymentStatus? status, PaymentType? type, int page, int pageSize)
        {
            var query = _context.Payments.Where(p => !p.IsDeleted).AsQueryable();

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            if (type.HasValue)
                query = query.Where(p => p.PaymentType == type.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Payment>(items, page, pageSize, totalCount);
        }

        public async Task<Payment> UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}
