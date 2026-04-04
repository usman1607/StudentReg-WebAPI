using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Course> AddAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Courses.AnyAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(Guid id)
        {
            return await _context.Courses
                .Where(c => c.Id == id && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Course>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.Courses
                .Where(c => ids.Contains(c.Id) && !c.IsDeleted)
                .ToListAsync();
        }
    }
}
