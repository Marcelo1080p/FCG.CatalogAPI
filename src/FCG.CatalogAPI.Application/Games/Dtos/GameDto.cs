using FCG.CatalogAPI.Domain.Entities;

namespace FCG.CatalogAPI.Application.Games.Dtos;

public record GameDto(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    decimal DiscountPercentage,
    decimal FinalPrice,
    bool IsActive,
    DateTime CreatedAt)
{
    public static GameDto FromEntity(Game game) => new(
        game.Id,
        game.Title,
        game.Description,
        game.Price,
        game.DiscountPercentage,
        game.FinalPrice,
        game.IsActive,
        game.CreatedAt);
}
