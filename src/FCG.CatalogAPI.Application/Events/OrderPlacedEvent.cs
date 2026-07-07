namespace FCG.Contracts.Events;

public record OrderPlacedEvent(Guid OrderId, Guid UserId, Guid GameId, decimal Amount);
