using Application.Dtos.Common;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IStudentRepository
    {
        Task<Student?> GetAsync(string matricNo);
        Task<Student?> GetByIdAsync(Guid id);
        Task<Student> CreateAsync(Student student);
        Task<List<Student>> GetAllAsync();
        Task<PagedResult<Student>> SearchAsync(string? searchTerm, int page, int pageSize, string? sortBy);
        Task<Student> UpdateAsync(Student student);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
