using Application.Dtos.RequestDto;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface ICourseService
    {
            Task<Course> CreateAsync(CourseRequestDto request);
            Task<Course> GetByIdAsync(Guid id);
            Task<IEnumerable<Course>> GetAllAsync();
            //Task<Course> UpdateAsync(Guid id, CourseRequestDto request, string updatedBy);
            Task<bool> DeleteAsync(Guid id);
    }
}
