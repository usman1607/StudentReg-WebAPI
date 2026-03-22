using Domain.Enums;

namespace Domain.Entities
{
    public class Staff : User
    {
        public string StaffNumber { get; set; } = default!;
        public StaffDelegation Delegation { get; set; }

        public Staff(
            string staffNumber,
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            string hashSalt,
            Gender gender,
            string phoneNumber,
            string address,
            StaffDelegation delegation,
            string createdBy, 
            string? profilePictureUrl = null)
            : base(firstName, lastName, email, passwordHash, hashSalt, gender, phoneNumber, address, UserType.Staff, profilePictureUrl)
        {
            StaffNumber = staffNumber;
            Delegation = delegation;
            CreatedBy = createdBy;
        }

        // EF Core parameterless constructor
        protected Staff() : base("", "", "", "", "", Gender.Male, "", "", UserType.Staff) { }
    }
}
