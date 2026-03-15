using Application.Dtos.RequestDto;
using Application.Services.Interfaces;
using Asp.Versioning;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        /// <summary>
        /// Get all students with optional search, filtering, and pagination
        /// </summary>
        /// <param name="searchTerm">Search by name, email, or matric number</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
        /// <param name="sortBy">Sort field (firstname, lastname, matricnumber, createddate) with optional _desc suffix</param>
        /// <returns>Paginated list of students</returns>
        [Authorize(Policy = "AdminOrInstructor")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm = null,
            [FromQuery] StudentStatus? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            _logger.LogInformation(
                "GET /students - SearchTerm: {SearchTerm}, Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}",
                searchTerm, page, pageSize, sortBy);

            var result = await _studentService.SearchAsync(searchTerm, status, page, pageSize, sortBy);
            return Ok(result);
        }

        /// <summary>
        /// Get a student by ID
        /// </summary>
        /// <param name="id">Student's unique identifier</param>
        /// <returns>Student details or 404 if not found</returns>
        [Authorize(Policy = "AdminOrInstructor")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            _logger.LogInformation("GET /students/{StudentId}", id);

            var student = await _studentService.GetByIdAsync(id);

            if (student == null)
            {
                _logger.LogWarning("Student with ID {StudentId} not found", id);
                return NotFound(new { message = $"Student with ID '{id}' was not found." });
            }

            return Ok(student);
        }

        /// <summary>
        /// Get a student by matriculation number
        /// </summary>
        /// <param name="matricNo">Student's matriculation number</param>
        /// <returns>Student details or 404 if not found</returns>
        [HttpGet("matric/{matricNo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByMatricNumber([FromRoute] string matricNo)
        {
            _logger.LogInformation("GET /students/matric/{MatricNumber}", matricNo);

            var student = await _studentService.GetByMatricAsync(matricNo);

            if (student == null)
            {
                _logger.LogWarning("Student with MatricNumber {MatricNumber} not found", matricNo);
                return NotFound(new { message = $"Student with matriculation number '{matricNo}' was not found." });
            }

            return Ok(student);
        }

        /// <summary>
        /// Create a new student
        /// </summary>
        /// <param name="request">Student creation request</param>
        /// <returns>Created student with 201 status code</returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] StudentRequestDto request)
        {
            _logger.LogInformation("POST /students - Creating student with email: {Email}", request.Email);

            var student = await _studentService.CreateAsync(request);

            _logger.LogInformation("Student created with ID: {StudentId}", student.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = student.Id, version = "1.0" },
                student);
        }

        /// <summary>
        /// Update an existing student
        /// </summary>
        /// <param name="id">Student's unique identifier</param>
        /// <param name="request">Update request with fields to modify</param>
        /// <returns>204 No Content on success, 404 if not found</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] StudentUpdateRequest request)
        {
            _logger.LogInformation("PUT /students/{StudentId}", id);

            var student = await _studentService.UpdateAsync(id, request);

            if (student == null)
            {
                _logger.LogWarning("Cannot update - Student with ID {StudentId} not found", id);
                return NotFound(new { message = $"Student with ID '{id}' was not found." });
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a student (soft delete)
        /// </summary>
        /// <param name="id">Student's unique identifier</param>
        /// <returns>204 No Content on success, 404 if not found</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            _logger.LogInformation("DELETE /students/{StudentId}", id);

            var result = await _studentService.DeleteAsync(id);

            if (!result)
            {
                _logger.LogWarning("Cannot delete - Student with ID {StudentId} not found", id);
                return NotFound(new { message = $"Student with ID '{id}' was not found." });
            }

            return NoContent();
        }

        /// <summary>
        /// Accept admission offer (Student owner only)
        /// </summary>
        /// <param name="id">Student's unique identifier</param>
        /// <returns>Updated student with Accepted status</returns>
        [HttpPost("{id:guid}/accept-offer")]
        [Authorize(Policy = "StudentOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AcceptOffer([FromRoute] Guid id)
        {
            _logger.LogInformation("POST /students/{StudentId}/accept-offer", id);

            // Verify the authenticated user is the owner of this student record
            var currentUserId = User.FindFirstValue("userId")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out var userId))
            {
                return Unauthorized(new { message = "User ID not found in token." });
            }

            if (userId != id)
            {
                _logger.LogWarning("User {UserId} attempted to accept offer for student {StudentId}", userId, id);
                return Forbid();
            }

            var student = await _studentService.AcceptAdmissionAsync(id);

            _logger.LogInformation("Student {StudentId} accepted admission offer", id);

            return Ok(student);
        }
    }
}
