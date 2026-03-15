using Domain.Enums;

namespace Application.Dtos.ResponseDto
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public string MatricNumber { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string Gender { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = default!;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}