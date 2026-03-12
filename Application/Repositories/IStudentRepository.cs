using Domain.Entities;

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
