using Application.Dtos.RequestDto;
using Application.Services.Contracts;
using Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumers
{
    public class StudentAdmissionAcceptedConsumer : IConsumer<StudentAdmissionAcceptedEvent>
    {
        private readonly IMailService _mailService;
        private readonly ILogger<StudentAdmissionAcceptedConsumer> _logger;

        public StudentAdmissionAcceptedConsumer(IMailService mailService, ILogger<StudentAdmissionAcceptedConsumer> logger)
        {
            _mailService = mailService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StudentAdmissionAcceptedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("Consumed StudentAdmissionAcceptedEvent for student {StudentId} ({Email})",
                evt.StudentId, evt.Email);

            var mailRequest = new MailRequest
            {
                To = evt.Email,
                ReceiverName = $"{evt.FirstName} {evt.LastName}",
                Subject = "Congratulations! Your Admission Has Been Confirmed",
                Body = $"""
                    <h2>Congratulations, {evt.FirstName}!</h2>
                    <p>Your admission has been officially accepted. Welcome to the institution!</p>
                    <ul>
                        <li><strong>Matric Number:</strong> {evt.MatricNumber}</li>
                        <li><strong>Acceptance Date:</strong> {evt.AcceptedAt:f}</li>
                    </ul>
                    <p>Please log in to your portal to view your assigned courses and next steps.</p>
                    """
            };

            await _mailService.SendEmailAsync(mailRequest);
            _logger.LogInformation("Admission confirmation email sent to {Email}", evt.Email);
        }
    }
}
