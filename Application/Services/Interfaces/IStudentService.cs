using Application.Dtos.Common;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;

namespace Application.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentDto?> GetByMatricAsync(string matricNo);
        Task<StudentDto?> GetByIdAsync(Guid id);
        Task<StudentDto> CreateAsync(StudentRequestDto request);
        Task<List<StudentDto>> GetAllAsync();
        Task<PagedResult<StudentDto>> SearchAsync(string? searchTerm, int page, int pageSize, string? sortBy);
        Task<StudentDto?> UpdateAsync(Guid id, StudentUpdateRequest updateRequest);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<StudentDto> AcceptAdmissionAsync(Guid studentId);
    }
}
