using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;

namespace Application.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentDto> GetAsync(string matricNo);
        Task<StudentDto> CreateAsync(StudentRequestDto request);
        Task<List<StudentDto>> GetAllAsync();
        StudentDto Update(Guid id, StudentUpdateRequest updateRequest);
    }
}
