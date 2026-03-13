using Domain.Enums;
using FluentValidation;

namespace Application.Dtos.RequestDto
{
    public class StudentUpdateRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public Gender? Gender { get; set; }
    }

    public class StudentUpdateRequestValidator : AbstractValidator<StudentUpdateRequest>
    {
        public StudentUpdateRequestValidator()
        {
            RuleFor(s => s.Email)
                .EmailAddress()
                .When(s => !string.IsNullOrEmpty(s.Email))
                .WithMessage("Use a valid email address");

            RuleFor(s => s.FirstName)
                .MinimumLength(2)
                .When(s => !string.IsNullOrEmpty(s.FirstName))
                .WithMessage("First name must be at least 2 characters");

            RuleFor(s => s.LastName)
                .MinimumLength(2)
                .When(s => !string.IsNullOrEmpty(s.LastName))
                .WithMessage("Last name must be at least 2 characters");
        }
    }
}
