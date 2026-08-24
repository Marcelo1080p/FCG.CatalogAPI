using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Games.Dtos;
using FCG.CatalogAPI.Application.Interfaces;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Queries.GetGameById;

public class GetGameByIdQueryHandler : IRequestHandler<GetGameByIdQuery, Result<GameDto>>
{
    private readonly IGameRepository _gameRepository;
    private readonly ICacheService _cache;

    public GetGameByIdQueryHandler(IGameRepository gameRepository, ICacheService cache)
    {
        _gameRepository = gameRepository;
        _cache = cache;
    }

    public async Task<Result<GameDto>> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        var key = CacheKeys.Game(request.Id);

        var cached = await _cache.GetAsync<GameDto>(key, cancellationToken);
        if (cached is not null)
            return Result<GameDto>.Success(cached);

        var game = await _gameRepository.GetByIdAsync(request.Id);
        if (game is null)
            return Result<GameDto>.Failure("Jogo não encontrado.");

        var dto = GameDto.FromEntity(game);
        await _cache.SetAsync(key, dto, cancellationToken: cancellationToken);

        return Result<GameDto>.Success(dto);
    }
}
