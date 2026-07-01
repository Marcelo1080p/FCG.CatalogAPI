using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Commands.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, Result<Guid>>
{
    private readonly IGameRepository _gameRepository;

    public CreateGameCommandHandler(IGameRepository gameRepository)
        => _gameRepository = gameRepository;

    public async Task<Result<Guid>> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var game = Game.Create(request.Title, request.Description, request.Price);
            await _gameRepository.AddAsync(game);
            await _gameRepository.SaveChangesAsync();
            return Result<Guid>.Success(game.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}
