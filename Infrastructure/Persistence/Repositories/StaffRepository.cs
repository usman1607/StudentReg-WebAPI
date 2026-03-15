using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly AppDbContext _context;

        public StaffRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Staff> CreateAsync(Staff staff)
        {
            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();
            return staff;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var staff = await GetByIdAsync(id);
            if (staff == null)
                return false;

            staff.IsDeleted = true;
            staff.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Staff.AnyAsync(s => s.Id == id && !s.IsDeleted);
        }

        public async Task<List<Staff>> GetAllAsync()
        {
            return await _context.Staff
                .Where(s => !s.IsDeleted)
                .Include(s => s.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<List<Staff>> GetByDelegationAsync(StaffDelegation delegation)
        {
            return await _context.Staff
                .Where(s => s.Delegation == delegation && !s.IsDeleted)
                .Include(s => s.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<Staff?> GetByEmailAsync(string email)
        {
            return await _context.Staff
                .Where(s => s.Email == email && !s.IsDeleted)
                .Include(s => s.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync();
        }

        public async Task<Staff?> GetByIdAsync(Guid id)
        {
            return await _context.Staff
                .Where(s => s.Id == id && !s.IsDeleted)
                .Include(s => s.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync();
        }

        public async Task<Staff?> GetByStaffNumberAsync(string staffNumber)
        {
            return await _context.Staff
                .Where(s => s.StaffNumber == staffNumber && !s.IsDeleted)
                .Include(s => s.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync();
        }

        public async Task<Staff> UpdateAsync(Staff staff)
        {
            staff.UpdatedDate = DateTime.UtcNow;
            _context.Staff.Update(staff);
            await _context.SaveChangesAsync();
            return staff;
        }
    }
}
