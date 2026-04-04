using Application.Dtos.RequestDto;
using Application.Helpers;
using Application.Repositories;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ILogger<CourseService> _logger;
        private readonly ICourseRepository _courseRepo;
        private readonly IAuthService _authService;

        public CourseService(ILogger<CourseService> logger, ICourseRepository courseRepo, IAuthService authService)
        {
            _logger = logger;
            _courseRepo = courseRepo;
            _authService = authService;
        }

        public async Task<Course> CreateAsync(CourseRequestDto request)
        {
            _logger.LogInformation("Creating course with name: {name}", request.Name);

            var createdBy = _authService.GetSignedInEmail();
            var courCode = CourseHelper.GenerateCourseCode(request.Name);
            var course = new Course
            {
                Name = request.Name,
                Description = request.Description,
                CreditUnits = request.CreditUnits,
                Code = courCode,
                CreatedBy = createdBy,
            };
            return await _courseRepo.AddAsync(course);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _courseRepo.GetAllAsync();
        }

        public Task<Course> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
