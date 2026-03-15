using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Exceptions;
using Application.Repositories;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IStudentRepository studentRepository,
            IJwtService jwtService,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _jwtService = jwtService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found: {Email}", request.Email);
                throw new ValidationException("Invalid email or password.");
            }

            // Verify password
            if (!VerifyPassword(request.Password, user.PasswordHash, user.HashSalt))
            {
                _logger.LogWarning("Login failed - invalid password for: {Email}", request.Email);
                throw new ValidationException("Invalid email or password.");
            }

            // Get user roles
            var roles = user.UserRoles
                .Where(ur => !ur.IsDeleted && ur.Role != null)
                .Select(ur => ur.Role.Name)
                .ToList();

            // Get delegation if user is Staff
            string? delegation = null;
            if (user is Staff staff)
            {
                delegation = staff.Delegation.ToString();
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user, roles, delegation);
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");

            _logger.LogInformation("Login successful for: {Email}", request.Email);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserType = user.UserType.ToString(),
                    Roles = roles,
                    Delegation = delegation
                }
            };
        }

        public async Task<AuthResponseDto> RegisterStudentAsync(StudentRequestDto request)
        {
            _logger.LogInformation("Registering new student with email: {Email}", request.Email);

            // Check if email already exists
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new ValidationException("A user with this email already exists.");
            }

            // Generate matriculation number
            var matricNumber = GenerateMatricNumber();

            // Create password hash
            var (hash, salt) = GeneratePasswordHash(request.Password);

            var student = new Student(
                matricNumber: matricNumber,
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                passwordHash: hash,
                hashSalt: salt,
                gender: request.Gender,
                phoneNumber: request.PhoneNo,
                address: request.Address,
                createdBy: "Self-Registration"
            );

            var createdStudent = await _studentRepository.CreateAsync(student);

            // Assign Student role
            // Note: Role assignment is handled separately via UserRole

            // Get roles (empty for new students until role is assigned)
            var roles = new List<string> { "Student" };

            // Generate JWT token
            var token = _jwtService.GenerateToken(createdStudent, roles);
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");

            _logger.LogInformation("Student registered successfully with ID: {StudentId}, MatricNumber: {MatricNumber}",
                createdStudent.Id, createdStudent.MatricNumber);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                User = new UserInfoDto
                {
                    Id = createdStudent.Id,
                    Email = createdStudent.Email,
                    FirstName = createdStudent.FirstName,
                    LastName = createdStudent.LastName,
                    UserType = createdStudent.UserType.ToString(),
                    Roles = roles,
                    Delegation = null
                }
            };
        }

        private static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var hash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes($"{password}{storedSalt}")));
            return hash == storedHash;
        }

        private static string GenerateMatricNumber()
        {
            var year = DateTime.UtcNow.Year;
            var random = new Random().Next(10000, 99999);
            return $"STU/{year}/{random}";
        }

        private static (string hash, string salt) GeneratePasswordHash(string password)
        {
            var salt = Guid.NewGuid().ToString("N")[..16];
            var hash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes($"{password}{salt}")));
            return (hash, salt);
        }
    }
}
