using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private AppDbContext _appDbContext;

        public StudentRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Student> CreateAsync(Student student)
        {
            _appDbContext.Students.Add(student);
            await _appDbContext.SaveChangesAsync();
            return student;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _appDbContext.Students.Include(s => s.UserRoles).ToListAsync();
        }

        public async Task<Student?> GetAsync(string matricNo)
        {
            var student = await _appDbContext.Students.Where(s => s.MatricNumber == matricNo).FirstOrDefaultAsync();
            return student;
        }

        public Student Update(Student student)
        {
            _appDbContext.Students.Update(student);
            return student;
        }
    }
}
