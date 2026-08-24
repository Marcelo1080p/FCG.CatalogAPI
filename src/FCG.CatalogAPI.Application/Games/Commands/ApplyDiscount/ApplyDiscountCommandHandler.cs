using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Interfaces;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Commands.ApplyDiscount;

public class ApplyDiscountCommandHandler : IRequestHandler<ApplyDiscountCommand, Result<bool>>
{
    private readonly IGameRepository _gameRepository;
    private readonly ICacheService _cache;

    public ApplyDiscountCommandHandler(IGameRepository gameRepository, ICacheService cache)
    {
        _gameRepository = gameRepository;
        _cache = cache;
    }

    public async Task<Result<bool>> Handle(ApplyDiscountCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId);
        if (game is null)
            return Result<bool>.Failure("Jogo não encontrado.");

        try
        {
            game.ApplyDiscount(request.Percentage);
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
