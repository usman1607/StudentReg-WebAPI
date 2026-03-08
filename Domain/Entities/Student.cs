using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Student: User
    {
        public string MatricNumber { get; set; } = default!;
        public Student(string matricNo, string firstName, string lastName, string email, string phoneNo, string address, string createdBy): base(firstName, lastName, email, phoneNo, address)
        {
            MatricNumber = matricNo;
            CreatedBy = createdBy;
        }
    }
}
