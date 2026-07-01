using FCG.CatalogAPI.Application.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FCG.CatalogAPI.Application.Consumers;

public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedEventConsumer> _logger;

    public UserCreatedEventConsumer(ILogger<UserCreatedEventConsumer> logger)
        => _logger = logger;

    public Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var evt = context.Message;
        _logger.LogInformation(
            "[CatalogAPI] Novo usuário registrado: {Name} ({Email}) — Id: {UserId}",
            evt.Name, evt.Email, evt.UserId);
        return Task.CompletedTask;
    }
}
