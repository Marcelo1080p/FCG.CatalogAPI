using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Interfaces;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Commands.UpdateGame;

public class UpdateGameCommandHandler : IRequestHandler<UpdateGameCommand, Result<bool>>
{
    private readonly IGameRepository _gameRepository;
    private readonly ICacheService _cache;

    public UpdateGameCommandHandler(IGameRepository gameRepository, ICacheService cache)
    {
        _gameRepository = gameRepository;
        _cache = cache;
    }

    public async Task<Result<bool>> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.Id);
        if (game is null)
            return Result<bool>.Failure("Jogo não encontrado.");

        try
        {
            game.Update(request.Title, request.Description, request.Price);
            await _gameRepository.UpdateAsync(game);
            await _gameRepository.SaveChangesAsync();

            await _cache.RemoveAsync(CacheKeys.AllGames, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Game(game.Id), cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (ArgumentException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
