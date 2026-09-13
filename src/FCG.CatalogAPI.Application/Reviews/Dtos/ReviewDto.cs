using FCG.CatalogAPI.Domain.Entities;

namespace FCG.CatalogAPI.Application.Reviews.Dtos;

public record ReviewDto(
    Guid Id,
    Guid GameId,
    Guid UserId,
    int Rating,
    string Comment,
    IReadOnlyList<string> Tags,
    DateTime CreatedAt)
{
    public static ReviewDto FromEntity(Review review) => new(
        review.Id,
        review.GameId,
        review.UserId,
        review.Rating,
        review.Comment,
        review.Tags,
        review.CreatedAt);
}
