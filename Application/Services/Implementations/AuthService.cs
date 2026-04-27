using Application.Configurations;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Exceptions;
using Application.Helpers;
using Application.Repositories;
using Application.Services.Contracts;
using Application.Services.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IFileService _fileService;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StorageSettings _storageSettings;

        public AuthService(
            IFileServiceFactory factory,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IStudentRepository studentRepository,
            IJwtService jwtService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthService> logger,
            IOptions<StorageSettings> settings)
        {
            _storageSettings = settings.Value;
            var type = Enum.Parse<FileServiceType>(_storageSettings.StorageType);
            _fileService = factory.Create(type);
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _studentRepository = studentRepository;
            _jwtService = jwtService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
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
            if (!UserHelper.VerifyPassword(request.Password, user.PasswordHash, user.HashSalt))
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
                    Delegation = delegation,
                    ProfilePictureUrl = user.ProfilePictureUrl
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
            var matricNumber = UserHelper.GenerateMatricNumber();

            // Create password hash
            var (hash, salt) = UserHelper.GeneratePasswordHash(request.Password);

            var picture = "";
            /*if (request.ProfilePicture != null && request.ProfilePicture.Length > 0)
            {
                using var stream = new MemoryStream();
                await request.ProfilePicture.CopyToAsync(stream);

                byte[] profilePictureBytes = stream.ToArray();
                picture = Convert.ToBase64String(profilePictureBytes);
            }*/

            if (request.ProfilePicture != null && request.ProfilePicture.Length > 0)
            {
                var fileName = request.ProfilePicture.ContentType.Split('/')[0];
                string contentType = request.ProfilePicture.ContentType.Split('/')[1];
                var name = $"{fileName}_{request.Email}_{Guid.NewGuid()}.{contentType}";
                picture = await _fileService.UploadFile(request.ProfilePicture, name);
            }

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
                createdBy: "Self-Registration",
                profilePictureUrl: picture
            );

            var role = await _roleRepository.GetByNameAsync(RoleNames.Student);
            if (role != null)
            {
                var userRole = new UserRole
                {
                    UserId = student.Id,
                    RoleId = role.Id,
                    CreatedBy = "System"
                };
                student.AddRole(userRole);
            }

            var createdStudent = await _studentRepository.CreateAsync(student);
            
            var roles = new List<string> { RoleNames.Student };

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
                    Delegation = null,
                    ProfilePictureUrl = picture
                }
            };
        }
                
        public string GetSignedInEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? "";
        }

        public Guid? GetSignedInUserId()
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }

        public bool IsStudent()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("userType")?.Value == UserType.Student.ToString();
        }
    }
}
