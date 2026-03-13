using Domain.Enums;

namespace Domain.Entities
{
    public class Instructor: User
    {
        public string StaffNumber { get; set; } = default!;
        public Instructor(string staffNumber, string firstName, string lastName, string email, string passwordHash, string hashSalt, string phoneNumber, string address, string createdBy) : base(firstName, lastName, email, passwordHash, hashSalt, phoneNumber, address, UserType.Instructor)
        {
            StaffNumber = staffNumber;
            CreatedBy = createdBy;
        }
    }
}
