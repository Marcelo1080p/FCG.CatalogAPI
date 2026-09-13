using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Reviews.Dtos;
using MediatR;

namespace FCG.CatalogAPI.Application.Reviews.Queries.GetGameReviews;

public record GetGameReviewsQuery(Guid GameId) : IRequest<Result<IList<ReviewDto>>>;
