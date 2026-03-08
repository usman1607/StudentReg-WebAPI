using Domain.Dtos.RequestDto;
using Domain.Dtos.ResponseDto;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IStudentRepository
    {
        Task<Student?> GetAsync(string matricNo);

        Task<Student> CreateAsync(Student student);
        Task<List<Student>> GetAllAsync();

        Student Update(Student student);
    }
}
