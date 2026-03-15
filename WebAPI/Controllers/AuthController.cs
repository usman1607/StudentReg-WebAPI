using Application.Dtos.RequestDto;
using Application.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT token and user info</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            _logger.LogInformation("POST /auth/login - Login attempt for: {Email}", request.Email);

            var result = await _authService.LoginAsync(request);

            _logger.LogInformation("Login successful for: {Email}", request.Email);

            return Ok(result);
        }

        /// <summary>
        /// Register a new student (public registration)
        /// </summary>
        /// <param name="request">Student registration data</param>
        /// <returns>JWT token and user info</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] StudentRequestDto request)
        {
            _logger.LogInformation("POST /auth/register - Registering student: {Email}", request.Email);
            
            var result = await _authService.RegisterStudentAsync(request);

            _logger.LogInformation("Student registered successfully: {Email}", request.Email);

            return CreatedAtAction(nameof(Login), result);
        }
    }
}
