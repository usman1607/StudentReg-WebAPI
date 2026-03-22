using Domain.Enums;

namespace Domain.Entities
{
    public class Student : User
    {
        public string MatricNumber { get; set; } = default!;
        public StudentStatus Status { get; set; } = StudentStatus.Pending;

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

        // EF Core parameterless constructor
        protected Student() : base("", "", "", "", "", Gender.Male, "", "", UserType.Student) { }
    }
}
