using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Interfaces;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Commands.DeleteGame;

public class DeleteGameCommandHandler : IRequestHandler<DeleteGameCommand, Result<bool>>
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameSearchRepository _searchRepository;
    private readonly ICacheService _cache;

    public DeleteGameCommandHandler(
        IGameRepository gameRepository,
        IGameSearchRepository searchRepository,
        ICacheService cache)
    {
        _gameRepository = gameRepository;
        _searchRepository = searchRepository;
        _cache = cache;
    }

    public async Task<Result<bool>> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.Id);
        if (game is null)
            return Result<bool>.Failure("Jogo não encontrado.");

        try
        {
            game.Deactivate();
            await _gameRepository.UpdateAsync(game);
            await _gameRepository.SaveChangesAsync();

            // O jogo foi desativado: sai do indice para nao aparecer nas buscas.
            await _searchRepository.RemoveAsync(game.Id, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.AllGames, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Game(game.Id), cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
