using Application.Dtos.RequestDto;
using Domain.Entities;

namespace Application.Services.Contracts
{
    public interface IBackgroundJobs
    {
        Task SendStudentApprovalEmail(MailRequest request);
        Task AssignCoursesToStudent(string matricNumber, List<Guid> courseIds);
    }
}
