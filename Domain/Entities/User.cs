using Domain.Enums;

namespace Domain.Entities
{
    public abstract class User : BaseEntity
    {
        private readonly List<UserRole> _userRoles = new();

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string HashSalt { get; set; }
        public UserType UserType { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }

        public ICollection<UserRole> UserRoles
        {
            get => _userRoles;
            private set
            {
                _userRoles.Clear();
                _userRoles.AddRange(value);
            }
        }

        public User(string firstName, string lastName, string email, string passwordHash, string hashSalt, Gender gender, string phoneNumber, string address, UserType userType)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
            HashSalt = hashSalt;
            Gender = gender;
            PhoneNumber = phoneNumber;
            Address = address;
            UserType = userType;
        }

        public void AddRole(UserRole userRole)
        {
            _userRoles.Add(userRole);
        }
    }
}
