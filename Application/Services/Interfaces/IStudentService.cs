using Domain.Dtos.RequestDto;
using Domain.Dtos.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
