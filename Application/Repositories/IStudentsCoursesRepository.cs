using Domain.Entities;

namespace Application.Repositories
{
    public interface IStudentsCoursesRepository
    {
        Task<List<StudentsCourses>> GetByStudentIdAsync(Guid studentId);
        Task<StudentsCourses> CreateAsync(StudentsCourses studentsCourses);
        Task<bool> ExistsAsync(Guid studentId, Guid courseId);
        Task CreateRangeAsync(IEnumerable<StudentsCourses> studentsCourses);
    }
}
