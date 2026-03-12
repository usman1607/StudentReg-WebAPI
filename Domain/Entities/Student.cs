using Domain.Enums;

namespace Domain.Entities
{
    public class Student: User
    {
        public string MatricNumber { get; set; } = default!;
        public Student(string matricNo, string firstName, string lastName, string email, string passwordHash, string hashSalt, string phoneNo, string address, string createdBy): base(firstName, lastName, email, passwordHash, hashSalt, phoneNo, address, UserType.Student)
        {
            MatricNumber = matricNo;
            CreatedBy = createdBy;
        }
    }
}
