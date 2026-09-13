using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using MediatR;

namespace FCG.CatalogAPI.Application.Reviews.Commands.CreateReview;

public record CreateReviewCommand(
    Guid GameId,
    Guid UserId,
    int Rating,
    string? Comment,
    IEnumerable<string>? Tags) : IRequest<Result<Guid>>;
