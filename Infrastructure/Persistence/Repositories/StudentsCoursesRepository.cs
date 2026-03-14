using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class StudentsCoursesRepository : IStudentsCoursesRepository
    {
        private readonly AppDbContext _context;

        public StudentsCoursesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StudentsCourses> CreateAsync(StudentsCourses studentsCourses)
        {
            _context.StudentsCourses.Add(studentsCourses);
            await _context.SaveChangesAsync();
            return studentsCourses;
        }

        public async Task CreateRangeAsync(IEnumerable<StudentsCourses> studentsCourses)
        {
            _context.StudentsCourses.AddRange(studentsCourses);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid studentId, Guid courseId)
        {
            return await _context.StudentsCourses
                .AnyAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId && !sc.IsDeleted);
        }

        public async Task<List<StudentsCourses>> GetByStudentIdAsync(Guid studentId)
        {
            return await _context.StudentsCourses
                .Where(sc => sc.StudentId == studentId && !sc.IsDeleted)
                .ToListAsync();
        }
    }
}
