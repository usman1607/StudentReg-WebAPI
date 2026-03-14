using FluentValidation;

namespace Application.Dtos.RequestDto
{
    public class AssignCoursesDto
    {
        public Guid StudentId { get; set; }
        public List<Guid> CourseIds { get; set; } = new();
    }

    public class AssignCoursesValidator : AbstractValidator<AssignCoursesDto>
    {
        public AssignCoursesValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required.");

            RuleFor(x => x.CourseIds)
                .NotEmpty().WithMessage("At least one course must be specified.")
                .Must(ids => ids.All(id => id != Guid.Empty))
                .WithMessage("Invalid course ID.");
        }
    }
}
