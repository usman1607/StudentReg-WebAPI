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
        private readonly IRoleRepository _roleRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentsCoursesRepository _studentsCoursesRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<StaffService> _logger;

        public StaffService(
            IRoleRepository roleRepository,
            IStaffRepository staffRepository,
            IStudentRepository studentRepository,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            ICourseRepository courseRepository,
            IStudentsCoursesRepository studentsCoursesRepository,
            IMapper mapper,
            ILogger<StaffService> logger)
        {
            _roleRepository = roleRepository;
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

            var roles = await _roleRepository.GetByIdsAsync(request.RoleIds);
            if (roles.Count != request.RoleIds.Count)
            {
                var existingRoleIds = roles.Select(r => r.Id).ToHashSet();
                var missingRoleIds = request.RoleIds.Where(id => !existingRoleIds.Contains(id)).ToList();
                throw new ValidationException($"Roles not found: {string.Join(", ", missingRoleIds)}");
            }

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

            foreach (var role in roles)
            {
                staff.UserRoles.Add(new UserRole
                {
                    RoleId = role.Id,
                    UserId = staff.Id,
                    CreatedBy = createdBy
                });
            }

            var createdStaff = await _staffRepository.CreateAsync(staff);

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
                Delegation = staff.Delegation.ToString(),
                Gender = staff.Gender.ToString(),
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
