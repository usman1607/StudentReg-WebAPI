using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Domain.Enums;

namespace Application.Services.Interfaces
{
    public interface IStaffService
    {
        // Staff management (Admin only)
        Task<StaffDto> CreateAsync(StaffRequestDto request, string createdBy);
        Task<StaffDto?> GetByIdAsync(Guid id);
        Task<List<StaffDto>> GetAllAsync();

        // Admin delegation operations
        Task<List<StudentDto>> GetPendingStudentsAsync();
        Task<StudentDto> ApproveStudentAsync(Guid studentId, Guid staffId);
        Task<StudentDto> RejectStudentAsync(Guid studentId, Guid staffId, string? reason);

        // Instructor delegation operations
        Task<List<StudentDto>> GetAcceptedStudentsAsync();
        Task AssignCoursesToStudentAsync(AssignCoursesDto request, Guid instructorId);
    }
}
