namespace FCG.CatalogAPI.Application.Events;

public record UserCreatedEvent(Guid UserId, string Name, string Email);
