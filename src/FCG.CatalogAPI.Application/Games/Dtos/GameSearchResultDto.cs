using FCG.CatalogAPI.Domain.Entities;

namespace FCG.CatalogAPI.Application.Games.Dtos;

public record GameSearchResultDto(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    decimal DiscountPercentage,
    decimal FinalPrice,
    double Score)
{
    public static GameSearchResultDto FromHit(GameSearchHit hit) => new(
        hit.GameId,
        hit.Title,
        hit.Description,
        hit.Price,
        hit.DiscountPercentage,
        hit.FinalPrice,
        Math.Round(hit.Score, 3));
}
