namespace FCG.CatalogAPI.Application.Events;

public record OrderPlacedEvent(Guid OrderId, Guid UserId, Guid GameId, decimal Amount);
