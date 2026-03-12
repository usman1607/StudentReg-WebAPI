using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.RequestDto
{
    public class StudentRequestDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string PhoneNo { get; set; } = default!;
        public DateOnly DateOfBirth { get; set; } = default!;
        //public int Age { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string Email { get; set; } = default!;
    }

    public class StudentRequestValidator : AbstractValidator<StudentRequestDto>
    {
        public StudentRequestValidator()
        {
            RuleFor(s => s.FirstName)
                .NotEmpty().WithMessage("FirstName is required");

            RuleFor(s => s.LastName)
                .NotEmpty().WithMessage("LastName is required.");

            RuleFor(s => s.Email)
                .EmailAddress().WithMessage("Use a valid email address");

            RuleFor(s => s.DateOfBirth)
                .Must(d => d.Year <= (DateTime.Now.Year - 18)).WithMessage("You must be at least 18 years old to register");

            //RuleFor(s => s.Age).GreaterThan(18).WithMessage("You must be at least 18 years old to register");
        }
    }
}
