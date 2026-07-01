using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Commands.UpdateGame;

public class UpdateGameCommandHandler : IRequestHandler<UpdateGameCommand, Result<bool>>
{
    private readonly IGameRepository _gameRepository;

    public UpdateGameCommandHandler(IGameRepository gameRepository)
        => _gameRepository = gameRepository;

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
            return Result<bool>.Success(true);
        }
        catch (ArgumentException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
