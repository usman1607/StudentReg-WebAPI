using Domain.Entities;

namespace Application.Repositories
{
    public interface ICourseRepository
    {
        Task<Course> AddAsync(Course course);
        Task<Course?> GetByIdAsync(Guid id);
        Task<List<Course>> GetByIdsAsync(IEnumerable<Guid> ids);
        Task<List<Course>> GetAllAsync();
        Task<bool> ExistsAsync(Guid id);
    }
}
