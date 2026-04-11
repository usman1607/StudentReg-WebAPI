using Application.Dtos.RequestDto;
using Application.Services.Contracts;
using Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumers
{
    public class StudentRegisteredConsumer : IConsumer<StudentRegisteredEvent>
    {
        private readonly IMailService _mailService;
        private readonly ILogger<StudentRegisteredConsumer> _logger;

        public StudentRegisteredConsumer(IMailService mailService, ILogger<StudentRegisteredConsumer> logger)
        {
            _mailService = mailService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StudentRegisteredEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("Consumed StudentRegisteredEvent for student {StudentId} ({Email})",
                evt.StudentId, evt.Email);

            var mailRequest = new MailRequest
            {
                To = evt.Email,
                ReceiverName = $"{evt.FirstName} {evt.LastName}",
                Subject = "Welcome to the Student Registration System",
                Body = $"""
                    <h2>Welcome, {evt.FirstName}!</h2>
                    <p>Your registration was successful. Here are your details:</p>
                    <ul>
                        <li><strong>Matric Number:</strong> {evt.MatricNumber}</li>
                        <li><strong>Email:</strong> {evt.Email}</li>
                        <li><strong>Registered At:</strong> {evt.RegisteredAt:f}</li>
                    </ul>
                    <p>Please log in to complete your profile and check your admission status.</p>
                    """
            };

            await _mailService.SendEmailAsync(mailRequest);
            _logger.LogInformation("Welcome email sent to {Email}", evt.Email);
        }
    }
}
