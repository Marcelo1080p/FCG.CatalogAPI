using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Reviews.Dtos;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Reviews.Queries.GetGameReviews;

public class GetGameReviewsQueryHandler : IRequestHandler<GetGameReviewsQuery, Result<IList<ReviewDto>>>
{
    private readonly IGameRepository _gameRepository;
    private readonly IReviewRepository _reviewRepository;

    public GetGameReviewsQueryHandler(
        IGameRepository gameRepository,
        IReviewRepository reviewRepository)
    {
        _gameRepository = gameRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<IList<ReviewDto>>> Handle(GetGameReviewsQuery request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId);
        if (game is null)
            return Result<IList<ReviewDto>>.Failure("Jogo não encontrado.");

        var reviews = await _reviewRepository.GetByGameIdAsync(request.GameId, cancellationToken);

        IList<ReviewDto> dtos = reviews.Select(ReviewDto.FromEntity).ToList();
        return Result<IList<ReviewDto>>.Success(dtos);
    }
}
