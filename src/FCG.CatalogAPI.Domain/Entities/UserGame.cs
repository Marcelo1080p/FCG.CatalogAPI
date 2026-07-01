namespace FCG.CatalogAPI.Domain.Entities;

public class UserGame
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public decimal PricePaid { get; private set; }
    public DateTime AcquiredAt { get; private set; }

    public Game Game { get; private set; } = null!;

    private UserGame() { }

    public static UserGame Create(Guid userId, Guid gameId, decimal pricePaid)
    {
        return new UserGame
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GameId = gameId,
            PricePaid = pricePaid,
            AcquiredAt = DateTime.UtcNow
        };
    }
}
