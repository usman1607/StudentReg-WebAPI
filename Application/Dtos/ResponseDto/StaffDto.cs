using Domain.Enums;

namespace Application.Dtos.ResponseDto
{
    public class StaffDto
    {
        public Guid Id { get; set; }
        public string StaffNumber { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Address { get; set; } = default!;
        public StaffDelegation Delegation { get; set; }
        public Gender Gender { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = default!;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
