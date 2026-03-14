using Domain.Entities;
using Domain.Enums;

namespace Application.Repositories
{
    public interface IStaffRepository
    {
        Task<Staff?> GetByIdAsync(Guid id);
        Task<Staff?> GetByEmailAsync(string email);
        Task<Staff?> GetByStaffNumberAsync(string staffNumber);
        Task<Staff> CreateAsync(Staff staff);
        Task<Staff> UpdateAsync(Staff staff);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<List<Staff>> GetByDelegationAsync(StaffDelegation delegation);
        Task<List<Staff>> GetAllAsync();
    }
}
