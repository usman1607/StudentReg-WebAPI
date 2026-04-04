using Application.Dtos.RequestDto;
using Application.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly ILogger<StaffController> _logger;

        public StaffController(IStaffService staffService, ILogger<StaffController> logger)
        {
            _staffService = staffService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new staff member (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] StaffRequestDto request)
        {
            var createdBy = User.FindFirstValue(ClaimTypes.Email) ?? "System";
            _logger.LogInformation("POST /staff - Creating staff with delegation: {Delegation}", request.Delegation);

            var staff = await _staffService.CreateAsync(request, createdBy);

            return CreatedAtAction(nameof(GetById), new { id = staff.Id, version = "1.0" }, staff);
        }

        /// <summary>
        /// Get staff by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var staff = await _staffService.GetByIdAsync(id);
            if (staff == null)
            {
                return NotFound(new { message = $"Staff with ID '{id}' was not found." });
            }
            return Ok(staff);
        }

        /// <summary>
        /// Get all staff members (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var staffList = await _staffService.GetAllAsync();
            return Ok(staffList);
        }

        // ==================== Admin Delegation Endpoints ====================

        /// <summary>
        /// Get all pending students (Admin delegation only)
        /// </summary>
        [HttpGet("students/pending")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingStudents()
        {
            _logger.LogInformation("GET /staff/students/pending");

            var students = await _staffService.GetPendingStudentsAsync();
            return Ok(students);
        }

        /// <summary>
        /// Approve a student's registration (Admin delegation only)
        /// </summary>
        [HttpPost("students/{studentId:guid}/approve")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveStudent([FromRoute] Guid studentId, [FromBody] List<Guid> courseIds)
        {
            var staffId = GetCurrentUserId();
            _logger.LogInformation("POST /staff/students/{StudentId}/approve by {StaffId}", studentId, staffId);

            var student = await _staffService.ApproveStudentAsync(studentId, staffId, courseIds);
            return Ok(student);
        }

        /// <summary>
        /// Reject a student's registration (Admin delegation only)
        /// </summary>
        [HttpPost("students/{studentId:guid}/reject")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectStudent([FromRoute] Guid studentId, [FromQuery] string? reason = null)
        {
            var staffId = GetCurrentUserId();
            _logger.LogInformation("POST /staff/students/{StudentId}/reject by {StaffId}", studentId, staffId);

            var student = await _staffService.RejectStudentAsync(studentId, staffId, reason);
            return Ok(student);
        }

        // ==================== Instructor Delegation Endpoints ====================

        /// <summary>
        /// Get all accepted students (Instructor delegation only)
        /// </summary>
        [HttpGet("students/accepted")]
        [Authorize(Policy = "InstructorOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAcceptedStudents()
        {
            _logger.LogInformation("GET /staff/students/accepted");

            var students = await _staffService.GetAcceptedStudentsAsync();
            return Ok(students);
        }

        /// <summary>
        /// Assign courses to a student (Instructor delegation only)
        /// </summary>
        [HttpPost("students/assign-courses")]
        [Authorize(Policy = "InstructorOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignCourses([FromBody] AssignCoursesDto request)
        {
            var instructorId = GetCurrentUserId();
            _logger.LogInformation("POST /staff/students/assign-courses by {InstructorId}", instructorId);

            await _staffService.AssignCoursesToStudentAsync(request, instructorId);
            return Ok(new { message = "Courses assigned successfully." });
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue("userId")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }

            return userId;
        }
    }
}
