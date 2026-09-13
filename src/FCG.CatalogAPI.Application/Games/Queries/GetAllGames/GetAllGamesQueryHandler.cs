using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Games.Dtos;
using FCG.CatalogAPI.Application.Interfaces;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Queries.GetAllGames;

public class GetAllGamesQueryHandler : IRequestHandler<GetAllGamesQuery, Result<IList<GameDto>>>
{
    private readonly IGameRepository _gameRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ICacheService _cache;

    public GetAllGamesQueryHandler(
        IGameRepository gameRepository,
        IReviewRepository reviewRepository,
        ICacheService cache)
    {
        _gameRepository = gameRepository;
        _reviewRepository = reviewRepository;
        _cache = cache;
    }

    public async Task<Result<IList<GameDto>>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetAsync<IList<GameDto>>(CacheKeys.AllGames, cancellationToken);
        if (cached is not null)
            return Result<IList<GameDto>>.Success(cached);

        // SQL Server entrega o catalogo; MongoDB entrega as notas agregadas.
        var games = await _gameRepository.GetAllAsync();
        var ratings = await _reviewRepository.GetRatingsAsync(
            games.Select(g => g.Id), cancellationToken);

        IList<GameDto> dtos = games
            .Select(g => GameDto.FromEntity(g, Rating(ratings, g.Id)))
            .ToList();

        await _cache.SetAsync(CacheKeys.AllGames, dtos, cancellationToken: cancellationToken);

        return Result<IList<GameDto>>.Success(dtos);
    }

    private static GameRating? Rating(IDictionary<Guid, GameRating> ratings, Guid gameId)
        => ratings.TryGetValue(gameId, out var rating) ? rating : null;
}
