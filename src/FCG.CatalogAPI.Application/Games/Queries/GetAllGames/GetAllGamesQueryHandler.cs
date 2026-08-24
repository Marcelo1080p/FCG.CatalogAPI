using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Games.Dtos;
using FCG.CatalogAPI.Application.Interfaces;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Queries.GetAllGames;

public class GetAllGamesQueryHandler : IRequestHandler<GetAllGamesQuery, Result<IList<GameDto>>>
{
    private readonly IGameRepository _gameRepository;
    private readonly ICacheService _cache;

    public GetAllGamesQueryHandler(IGameRepository gameRepository, ICacheService cache)
    {
        _gameRepository = gameRepository;
        _cache = cache;
    }

    public async Task<Result<IList<GameDto>>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetAsync<IList<GameDto>>(CacheKeys.AllGames, cancellationToken);
        if (cached is not null)
            return Result<IList<GameDto>>.Success(cached);

        var games = await _gameRepository.GetAllAsync();
        var dtos = games.Select(GameDto.FromEntity).ToList();

        await _cache.SetAsync<IList<GameDto>>(CacheKeys.AllGames, dtos, cancellationToken: cancellationToken);

        return Result<IList<GameDto>>.Success(dtos);
    }
}
