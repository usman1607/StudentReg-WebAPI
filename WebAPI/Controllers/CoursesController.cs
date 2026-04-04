using Application.Dtos.RequestDto;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class CoursesController : Controller
    {

        private readonly ICourseService _courseService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] CourseRequestDto request)
        {
            _logger.LogInformation("POST /courses - Creating course with name: {name}", request.Name);

            var course = await _courseService.CreateAsync(request);

            return CreatedAtAction(nameof(Create), new { id = course.Id, version = "1.0" }, course);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrInstructor")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /courses - Retrieving all courses");
            var courses = await _courseService.GetAllAsync();
            return Ok(courses);
        }
    }
}
