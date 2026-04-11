using Application.Dtos.RequestDto;
using Application.Repositories;
using Application.Services.Contracts;
using Domain.Entities;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class MyBackgroundJobs : IBackgroundJobs
    {
        private readonly IMailService _mailService;
        private readonly ILogger<MyBackgroundJobs> _logger;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentsCoursesRepository _studentsCoursesRepository;
        private readonly IEventPublisher _eventPublisher;

        public MyBackgroundJobs(
            IMailService mailService,
            ILogger<MyBackgroundJobs> logger,
            ICourseRepository courseRepository,
            IStudentRepository studentRepository,
            IStudentsCoursesRepository studentsCoursesRepository,
            IEventPublisher eventPublisher)
        {
            _logger = logger;
            _mailService = mailService;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _studentsCoursesRepository = studentsCoursesRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task AssignCoursesToStudent(string email, List<Guid> courseIds)
        {
            var student = await _studentRepository.GetByEmailAsync(email);
            if (student == null)
            {
                _logger.LogWarning("Student with email: {Email} not found for course assignment", email);
                return;
            }

            var allCourses = new List<StudentsCourses>();
            var courses = await _courseRepository.GetByIdsAsync(courseIds);

            foreach (var course in courses)
            {
                var studentCourses = new StudentsCourses
                {
                    CourseId = course.Id,
                    StudentId = student.Id,
                    CreatedBy = "System"
                };
                allCourses.Add(studentCourses);
            }

            if(allCourses.Count == 0)
            {
                _logger.LogWarning("No valid courses found for assignment to student with email: {Email}", email);
                return;
            }
            await _studentsCoursesRepository.CreateRangeAsync(allCourses);
            student.AddCourses(allCourses);
            try
            {
                await _studentRepository.UpdateAsync(student);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
            _logger.LogInformation("Assigned courses to student with email: {Email}", email);

            await _eventPublisher.PublishAsync(new CourseAssignedEvent(
                StudentId: student.Id,
                StudentEmail: student.Email,
                MatricNumber: student.MatricNumber,
                CourseIds: courseIds,
                CourseCount: allCourses.Count,
                AssignedAt: DateTime.UtcNow));
        }

        public async Task SendStudentApprovalEmail(MailRequest mailRequest)
        {
            _logger.LogInformation("Sending student approval email to: {Email}", mailRequest.To);
            await _mailService.SendEmailAsync(mailRequest);
            _logger.LogInformation("Email sent successfully to: {Email}", mailRequest.To);
        }
    }
}
