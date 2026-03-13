using Domain.Enums;

namespace Domain.Entities
{
    public class Student: User
    {
        public string MatricNumber { get; set; } = default!;
        public Student(string matricNumber, string firstName, string lastName, string email, string passwordHash, string hashSalt, string phoneNumber, string address, string createdBy): base(firstName, lastName, email, passwordHash, hashSalt, phoneNumber, address, UserType.Student)
        {
            MatricNumber = matricNumber;
            CreatedBy = createdBy;
        }
    }
}
