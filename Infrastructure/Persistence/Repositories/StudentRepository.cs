using Application.Dtos.Common;
using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _appDbContext;

        public StudentRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Student> CreateAsync(Student student)
        {
            _appDbContext.Students.Add(student);
            await _appDbContext.SaveChangesAsync();
            return student;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _appDbContext.Students
                .Where(s => !s.IsDeleted)
                .Include(s => s.UserRoles)
                .ToListAsync();
        }

        public async Task<Student?> GetAsync(string matricNo)
        {
            return await _appDbContext.Students
                .Where(s => s.MatricNumber == matricNo && !s.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Student?> GetByIdAsync(Guid id)
        {
            return await _appDbContext.Students
                .Where(s => s.Id == id && !s.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<Student>> SearchAsync(string? searchTerm, int page, int pageSize, string? sortBy)
        {
            var query = _appDbContext.Students.Where(s => !s.IsDeleted).AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(s =>
                    s.FirstName.ToLower().Contains(term) ||
                    s.LastName.ToLower().Contains(term) ||
                    s.Email.ToLower().Contains(term) ||
                    s.MatricNumber.ToLower().Contains(term));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "firstname" => query.OrderBy(s => s.FirstName),
                "firstname_desc" => query.OrderByDescending(s => s.FirstName),
                "lastname" => query.OrderBy(s => s.LastName),
                "lastname_desc" => query.OrderByDescending(s => s.LastName),
                "matricnumber" => query.OrderBy(s => s.MatricNumber),
                "matricnumber_desc" => query.OrderByDescending(s => s.MatricNumber),
                "createddate" => query.OrderBy(s => s.CreatedDate),
                "createddate_desc" => query.OrderByDescending(s => s.CreatedDate),
                _ => query.OrderByDescending(s => s.CreatedDate)
            };

            // Apply pagination
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Student>(items, page, pageSize, totalCount);
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            student.UpdatedDate = DateTime.UtcNow;
            _appDbContext.Students.Update(student);
            await _appDbContext.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var student = await GetByIdAsync(id);
            if (student == null)
                return false;

            // Soft delete
            student.IsDeleted = true;
            student.UpdatedDate = DateTime.UtcNow;
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _appDbContext.Students.AnyAsync(s => s.Id == id && !s.IsDeleted);
        }
    }
}
