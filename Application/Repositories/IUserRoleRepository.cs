using Domain.Entities;

namespace Application.Repositories
{
    public interface IUserRoleRepository
    {
        Task<UserRole?> GetAsync(Guid userId, Guid roleId);
        Task<List<UserRole>> GetByUserIdAsync(Guid userId);
        Task<UserRole> CreateAsync(UserRole userRole);
        Task<bool> ExistsAsync(Guid userId, Guid roleId);
    }
}
