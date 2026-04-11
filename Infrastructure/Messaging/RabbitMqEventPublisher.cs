using Application.Services.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging
{
    public class RabbitMqEventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<RabbitMqEventPublisher> _logger;

        public RabbitMqEventPublisher(IPublishEndpoint publishEndpoint, ILogger<RabbitMqEventPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            _logger.LogInformation("Publishing event {EventType}", typeof(T).Name);
            await _publishEndpoint.Publish(message, cancellationToken);
            _logger.LogInformation("Event {EventType} published successfully", typeof(T).Name);
        }
    }
}
