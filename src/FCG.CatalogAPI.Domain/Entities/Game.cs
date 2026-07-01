namespace FCG.CatalogAPI.Domain.Entities;

public class Game
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Game() { }

    public static Game Create(string title, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Título é obrigatório.");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Descrição é obrigatória.");
        if (price < 0)
            throw new ArgumentException("Preço não pode ser negativo.");

        return new Game
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = description.Trim(),
            Price = price,
            DiscountPercentage = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string title, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Título é obrigatório.");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Descrição é obrigatória.");
        if (price < 0)
            throw new ArgumentException("Preço não pode ser negativo.");

        Title = title.Trim();
        Description = description.Trim();
        Price = price;
    }

    public void ApplyDiscount(decimal percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Desconto deve ser entre 0 e 100.");
        DiscountPercentage = percentage;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("Jogo já está inativo.");
        IsActive = false;
    }

    public decimal FinalPrice => Price * (1 - DiscountPercentage / 100);
}
