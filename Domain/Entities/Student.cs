using Domain.Enums;

namespace Domain.Entities
{
    public class Student : User
    {
        private readonly List<StudentsCourses> _studentCourses = new();
        public string MatricNumber { get; set; } = default!;
        public StudentStatus Status { get; set; } = StudentStatus.Pending;
        public ICollection<StudentsCourses> StudentsCourses
        {
            get => _studentCourses;
            private set
            {
                _studentCourses.Clear();
                _studentCourses.AddRange(value);
            }
        }

        public Student(
            string matricNumber,
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            string hashSalt,
            Gender gender,
            string phoneNumber,
            string address,
            string createdBy,
            string? profilePictureUrl = null)
            : base(firstName, lastName, email, passwordHash, hashSalt, gender, phoneNumber, address, UserType.Student, profilePictureUrl)
        {
            MatricNumber = matricNumber;
            Status = StudentStatus.Pending;
            CreatedBy = createdBy;
        }

        public void AddCourses(List<StudentsCourses> studentsCourses)
        {
            _studentCourses.AddRange(studentsCourses);
        }

        // EF Core parameterless constructor
        protected Student() : base("", "", "", "", "", Gender.Male, "", "", UserType.Student) { }
    }
}
