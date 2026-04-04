using FluentValidation;

namespace Application.Dtos.RequestDto
{
    public class CourseRequestDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public int CreditUnits { get; set; }
    }

    public class CourseRequestDtoValidator: AbstractValidator<CourseRequestDto>
    {
        public CourseRequestDtoValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(c => c.CreditUnits)
                .GreaterThan(0).WithMessage("Credit Units must be greater than 0");
        }
    }
}
