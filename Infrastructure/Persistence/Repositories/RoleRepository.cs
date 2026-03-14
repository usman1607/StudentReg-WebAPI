using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Roles
                .Where(r => !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(Guid id)
        {
            return await _context.Roles
                .Where(r => r.Id == id && !r.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Roles
                .Where(r => r.Name == name && !r.IsDeleted)
                .FirstOrDefaultAsync();
        }
    }
}
