using Application.Dtos.RequestDto;
using SendGrid;

namespace Application.Services.Contracts
{
    public interface IMailService
    {
        Task<Response> SendEmailAsync(MailRequest request);
    }
}
