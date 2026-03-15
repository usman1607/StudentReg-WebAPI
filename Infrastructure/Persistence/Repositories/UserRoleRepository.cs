using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbContext _context;

        public UserRoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserRole> CreateAsync(UserRole userRole)
        {
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
            return userRole;
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid roleId)
        {
            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsDeleted);
        }

        public async Task<UserRole?> GetAsync(Guid userId, Guid roleId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsDeleted)
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserRole>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                .Include(ur => ur.Role)
                .ToListAsync();
        }
    }
}
