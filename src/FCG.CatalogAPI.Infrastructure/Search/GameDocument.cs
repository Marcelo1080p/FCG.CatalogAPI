using FCG.CatalogAPI.Domain.Entities;

namespace FCG.CatalogAPI.Infrastructure.Search;

/// <summary>
/// Representacao do jogo dentro do indice de busca. Guarda apenas o que
/// interessa para pesquisar e exibir o resultado.
/// </summary>
public class GameDocument
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal FinalPrice { get; set; }
    public bool IsActive { get; set; }

    public static GameDocument FromEntity(Game game) => new()
    {
        Id = game.Id.ToString(),
        Title = game.Title,
        Description = game.Description,
        Price = game.Price,
        DiscountPercentage = game.DiscountPercentage,
        FinalPrice = game.FinalPrice,
        IsActive = game.IsActive
    };

    public GameSearchHit ToSearchHit(double score) => new(
        Guid.Parse(Id),
        Title,
        Description,
        Price,
        DiscountPercentage,
        FinalPrice,
        score);
}
