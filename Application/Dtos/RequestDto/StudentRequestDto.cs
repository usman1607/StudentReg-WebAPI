using Domain.Enums;
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
        public Gender Gender { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
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

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");

            RuleFor(s => s.DateOfBirth)
                .Must(d => d.Year <= (DateTime.Now.Year - 18)).WithMessage("You must be at least 18 years old to register");

            RuleFor(s => s.Gender)
                .IsInEnum().WithMessage("Invalid gender.");

            //RuleFor(s => s.Age).GreaterThan(18).WithMessage("You must be at least 18 years old to register");
        }
    }
}
