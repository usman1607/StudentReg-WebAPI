using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User: BaseEntity
    {
        private readonly List<UserRole> _userRoles = new();

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string HashSalt { get; set; }
        public UserType UserType { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }
        public IList<UserRole> UserRoles
        {
			get => _userRoles.AsReadOnly();
			private set => _userRoles.AddRange(value);
		}

        public User(string firstName, string lastName, string email, string password, string passwordHash, string hashSalt, string phoneNo, string address, UserType userType)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            PasswordHash = passwordHash;
            HashSalt = 
            PhoneNumber = phoneNo;
            Address = address;
            UserType = userType;
        }
    }
}
