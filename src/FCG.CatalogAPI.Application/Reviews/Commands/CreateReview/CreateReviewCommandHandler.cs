using FCG.CatalogAPI.Application.Games;
using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Interfaces;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<Guid>>
{
    private readonly IGameRepository _gameRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ICacheService _cache;

    public CreateReviewCommandHandler(
        IGameRepository gameRepository,
        IReviewRepository reviewRepository,
        ICacheService cache)
    {
        _gameRepository = gameRepository;
        _reviewRepository = reviewRepository;
        _cache = cache;
    }

    public async Task<Result<Guid>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId);
        if (game is null)
            return Result<Guid>.Failure("Jogo não encontrado.");

        var alreadyReviewed = await _reviewRepository.ExistsAsync(
            request.GameId, request.UserId, cancellationToken);

        if (alreadyReviewed)
            return Result<Guid>.Failure("Usuário já avaliou este jogo.");

        try
        {
            var review = Review.Create(
                request.GameId, request.UserId, request.Rating, request.Comment, request.Tags);

            await _reviewRepository.AddAsync(review, cancellationToken);

            // A nota media faz parte do GameDto: o cache do catalogo precisa ser renovado.
            await _cache.RemoveAsync(CacheKeys.AllGames, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Game(request.GameId), cancellationToken);

            return Result<Guid>.Success(review.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}
