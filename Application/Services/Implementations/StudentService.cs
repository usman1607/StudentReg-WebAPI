using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Services.Interfaces;

namespace Application.Services.Implementations
{
    internal class StudentService : IStudentService
    {
        public Task<StudentDto> CreateAsync(StudentRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<List<StudentDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<StudentDto> GetAsync(string matricNo)
        {
            throw new NotImplementedException();
        }

        public StudentDto Update(Guid id, StudentUpdateRequest updateRequest)
        {
            throw new NotImplementedException();
        }
    }
}
