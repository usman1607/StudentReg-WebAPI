using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Instructor: User
    {
        public string StaffNumber { get; set; } = default!;
        public Instructor(string staffNumber, string firstName, string lastName, string email, string phoneNo, string address, string createdBy) : base(firstName, lastName, email, phoneNo, address)
        {
            StaffNumber = staffNumber;
            CreatedBy = createdBy;
        }
    }
}
