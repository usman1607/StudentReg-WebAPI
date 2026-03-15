using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Exceptions;
using Application.Helpers;
using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentsCoursesRepository _studentsCoursesRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<StaffService> _logger;

        // Role IDs (matching RoleSeeder)
        private static readonly Guid AdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid StudentRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid InstructorRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        public StaffService(
            IStaffRepository staffRepository,
            IStudentRepository studentRepository,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            ICourseRepository courseRepository,
            IStudentsCoursesRepository studentsCoursesRepository,
            IMapper mapper,
            ILogger<StaffService> logger)
        {
            _staffRepository = staffRepository;
            _studentRepository = studentRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _courseRepository = courseRepository;
            _studentsCoursesRepository = studentsCoursesRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<StaffDto> CreateAsync(StaffRequestDto request, string createdBy)
        {
            _logger.LogInformation("Creating new staff with email: {Email}, delegation: {Delegation}",
                request.Email, request.Delegation);

            // Check if email already exists
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new ValidationException("A user with this email already exists.");
            }

            // Generate staff number
            var staffNumber = UserHelper.GenerateStaffNumber(request.Delegation);

            // Create password hash
            var (hash, salt) = UserHelper.GeneratePasswordHash(request.Password);

            var staff = new Staff(
                staffNumber: staffNumber,
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                passwordHash: hash,
                hashSalt: salt,
                gender: request.Gender,
                phoneNumber: request.PhoneNumber,
                address: request.Address,
                delegation: request.Delegation,
                createdBy: createdBy
            );

            var createdStaff = await _staffRepository.CreateAsync(staff);

            // Assign appropriate role based on delegation
            var roleId = request.Delegation switch
            {
                StaffDelegation.Admin => AdminRoleId,
                StaffDelegation.Instructor => InstructorRoleId,
                _ => (Guid?)null
            };

            if (roleId.HasValue)
            {
                var userRole = new UserRole
                {
                    UserId = createdStaff.Id,
                    RoleId = roleId.Value,
                    CreatedBy = createdBy
                };
                await _userRoleRepository.CreateAsync(userRole);
            }

            _logger.LogInformation("Staff created successfully with ID: {StaffId}, StaffNumber: {StaffNumber}",
                createdStaff.Id, createdStaff.StaffNumber);

            return MapToDto(createdStaff);
        }

        public async Task<StaffDto?> GetByIdAsync(Guid id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            return staff == null ? null : MapToDto(staff);
        }

        public async Task<List<StaffDto>> GetAllAsync()
        {
            var staffList = await _staffRepository.GetAllAsync();
            return staffList.Select(MapToDto).ToList();
        }

        public async Task<List<StudentDto>> GetPendingStudentsAsync()
        {
            _logger.LogInformation("Fetching pending students");
            var students = await _studentRepository.GetByStatusAsync(StudentStatus.Pending);
            return _mapper.Map<List<StudentDto>>(students);
        }

        public async Task<StudentDto> ApproveStudentAsync(Guid studentId, Guid staffId)
        {
            _logger.LogInformation("Approving student {StudentId} by staff {StaffId}", studentId, staffId);

            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
            {
                throw new EntityNotFoundException($"Student with ID '{studentId}' not found.");
            }

            if (student.Status != StudentStatus.Pending)
            {
                throw new ValidationException($"Student status must be Pending to approve. Current status: {student.Status}");
            }

            var staff = await _staffRepository.GetByIdAsync(staffId);
            if (staff == null || staff.Delegation != StaffDelegation.Admin)
            {
                throw new ValidationException("Only Admin staff can approve students.");
            }

            student.Status = StudentStatus.OfferedAdmission;
            student.UpdatedBy = staff.Email;
            student.UpdatedDate = DateTime.UtcNow;

            await _studentRepository.UpdateAsync(student);

            // Assign Student role if not already assigned
            var roleExists = await _userRoleRepository.ExistsAsync(studentId, StudentRoleId);
            if (!roleExists)
            {
                var userRole = new UserRole
                {
                    UserId = studentId,
                    RoleId = StudentRoleId,
                    CreatedBy = staff.Email
                };
                await _userRoleRepository.CreateAsync(userRole);
            }

            _logger.LogInformation("Student {StudentId} approved. Status changed to OfferedAdmission", studentId);

            return _mapper.Map<StudentDto>(student);
        }

        public async Task<StudentDto> RejectStudentAsync(Guid studentId, Guid staffId, string? reason)
        {
            _logger.LogInformation("Rejecting student {StudentId} by staff {StaffId}", studentId, staffId);

            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
            {
                throw new EntityNotFoundException($"Student with ID '{studentId}' not found.");
            }

            if (student.Status != StudentStatus.Pending)
            {
                throw new ValidationException($"Student status must be Pending to reject. Current status: {student.Status}");
            }

            var staff = await _staffRepository.GetByIdAsync(staffId);
            if (staff == null || staff.Delegation != StaffDelegation.Admin)
            {
                throw new ValidationException("Only Admin staff can reject students.");
            }

            student.Status = StudentStatus.Rejected;
            student.UpdatedBy = staff.Email;
            student.UpdatedDate = DateTime.UtcNow;

            await _studentRepository.UpdateAsync(student);

            _logger.LogInformation("Student {StudentId} rejected. Reason: {Reason}", studentId, reason ?? "No reason provided");

            return _mapper.Map<StudentDto>(student);
        }

        public async Task<List<StudentDto>> GetAcceptedStudentsAsync()
        {
            _logger.LogInformation("Fetching accepted students");
            var students = await _studentRepository.GetByStatusAsync(StudentStatus.Accepted);
            return _mapper.Map<List<StudentDto>>(students);
        }

        public async Task AssignCoursesToStudentAsync(AssignCoursesDto request, Guid instructorId)
        {
            _logger.LogInformation("Assigning courses to student {StudentId} by instructor {InstructorId}",
                request.StudentId, instructorId);

            var student = await _studentRepository.GetByIdAsync(request.StudentId);
            if (student == null)
            {
                throw new EntityNotFoundException($"Student with ID '{request.StudentId}' not found.");
            }

            if (student.Status != StudentStatus.Accepted)
            {
                throw new ValidationException($"Student must have Accepted status to assign courses. Current status: {student.Status}");
            }

            var instructor = await _staffRepository.GetByIdAsync(instructorId);
            if (instructor == null || instructor.Delegation != StaffDelegation.Instructor)
            {
                throw new ValidationException("Only Instructor staff can assign courses.");
            }

            // Verify all courses exist
            var courseIds = request.CourseIds.Distinct().ToList();
            var existingCourses = await _courseRepository.GetByIdsAsync(courseIds);
            var existingCourseIds = existingCourses.Select(c => c.Id).ToList();

            var missingCourses = courseIds.Except(existingCourseIds).ToList();
            if (missingCourses.Any())
            {
                throw new ValidationException($"Courses not found: {string.Join(", ", missingCourses)}");
            }

            // Get existing assignments
            var existingAssignments = await _studentsCoursesRepository.GetByStudentIdAsync(request.StudentId);
            var existingCourseAssignments = existingAssignments.Select(sc => sc.CourseId).ToList();

            // Add only new assignments
            var newAssignments = courseIds.Except(existingCourseAssignments).ToList();
            if (newAssignments.Any())
            {
                var studentsCourses = newAssignments.Select(courseId => new StudentsCourses
                {
                    StudentId = request.StudentId,
                    CourseId = courseId,
                    CreatedBy = instructor.Email
                });

                await _studentsCoursesRepository.CreateRangeAsync(studentsCourses);
            }

            _logger.LogInformation("Assigned {Count} courses to student {StudentId}",
                newAssignments.Count, request.StudentId);
        }

        private static StaffDto MapToDto(Staff staff)
        {
            return new StaffDto
            {
                Id = staff.Id,
                StaffNumber = staff.StaffNumber,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Email = staff.Email,
                PhoneNumber = staff.PhoneNumber,
                Address = staff.Address,
                Delegation = staff.Delegation,
                Gender = staff.Gender,
                Roles = staff.UserRoles
                    .Where(ur => ur.Role != null)
                    .Select(ur => ur.Role.Name)
                    .ToList(),
                CreatedDate = staff.CreatedDate,
                CreatedBy = staff.CreatedBy,
                UpdatedDate = staff.UpdatedDate,
                UpdatedBy = staff.UpdatedBy
            };
        }
    }
}
