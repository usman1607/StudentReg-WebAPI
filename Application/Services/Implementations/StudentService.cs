using Application.Repositories;
using Application.Services.Interfaces;
using Domain.Dtos.RequestDto;
using Domain.Dtos.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return new StudentDto();
        }
    }
}
