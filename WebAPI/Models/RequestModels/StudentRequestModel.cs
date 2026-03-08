using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
    public class StudentRequestModel
    {
        [Required]
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string PhoneNo { get; set; } = default!;
        public DateOnly DateOfBirth { get; set; } = default!;
        [Length(18, 80)]
        public int Age { get; set; } = default!;
        public string Address { get; set; } = default!;       
        public string Email { get; set; } = default!;
    }

    public class StudentRequestValidator : AbstractValidator<StudentRequestModel>
    {
        public StudentRequestValidator()
        {
            RuleFor(s => s.FirstName)
                .NotEmpty().WithMessage("FirstName is required");

            RuleFor(s => s.LastName).NotEmpty().WithMessage("LastName is required.");
            RuleFor(s => s.Email).EmailAddress().WithMessage("Use a valid email address");
            
            RuleFor(s => s.DateOfBirth)
                .Must(d => d.Year <= (DateTime.Now.Year - 18)).WithMessage("You must be at least 18 years old to register");
            
            RuleFor(s => s.Age).GreaterThan(18).WithMessage("You must be at least 18 years old to register");
        }

        private int GetLeastYear()
        {
            var currentYear = DateTime.Now.Year;
            return currentYear - 18;
        }
    }
}
