using Domain.Entities;

namespace Application.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid id);
        Task<Role?> GetByNameAsync(string name);
        Task<List<Role>> GetByIdsAsync(IEnumerable<Guid> ids);
        Task<List<Role>> GetAllAsync();
    }
}
