using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Commands.ApplyDiscount;

public class ApplyDiscountCommandHandler : IRequestHandler<ApplyDiscountCommand, Result<bool>>
{
    private readonly IGameRepository _gameRepository;

    public ApplyDiscountCommandHandler(IGameRepository gameRepository)
        => _gameRepository = gameRepository;

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
            return Result<bool>.Success(true);
        }
        catch (ArgumentException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
