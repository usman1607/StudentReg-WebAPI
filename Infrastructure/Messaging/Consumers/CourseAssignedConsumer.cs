using Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumers
{
    public class CourseAssignedConsumer : IConsumer<CourseAssignedEvent>
    {
        private readonly ILogger<CourseAssignedConsumer> _logger;

        public CourseAssignedConsumer(ILogger<CourseAssignedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CourseAssignedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation(
                "Consumed CourseAssignedEvent: {CourseCount} course(s) assigned to student {MatricNumber} ({Email}) at {AssignedAt}",
                evt.CourseCount, evt.MatricNumber, evt.StudentEmail, evt.AssignedAt);

            // Extend here: e.g. send email notification, update a read model, trigger timetable generation, etc.
            await Task.CompletedTask;
        }
    }
}
