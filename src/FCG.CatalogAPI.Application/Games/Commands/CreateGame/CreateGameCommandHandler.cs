using FCG.CatalogAPI.Application.Interfaces;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Commands.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, Result<Guid>>
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameSearchRepository _searchRepository;
    private readonly ICacheService _cache;

    public CreateGameCommandHandler(
        IGameRepository gameRepository,
        IGameSearchRepository searchRepository,
        ICacheService cache)
    {
        _gameRepository = gameRepository;
        _searchRepository = searchRepository;
        _cache = cache;
    }

    public async Task<Result<Guid>> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var game = Game.Create(request.Title, request.Description, request.Price);
            await _gameRepository.AddAsync(game);
            await _gameRepository.SaveChangesAsync();

            await _searchRepository.IndexAsync(game, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.AllGames, cancellationToken);

            return Result<Guid>.Success(game.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}
