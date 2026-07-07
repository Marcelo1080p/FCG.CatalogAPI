using FCG.Contracts.Events;
using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using MassTransit;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Commands.AcquireGame;

public class AcquireGameCommandHandler : IRequestHandler<AcquireGameCommand, Result<Guid>>
{
    private readonly IGameRepository _gameRepository;
    private readonly IUserGameRepository _userGameRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public AcquireGameCommandHandler(
        IGameRepository gameRepository,
        IUserGameRepository userGameRepository,
        IPublishEndpoint publishEndpoint)
    {
        _gameRepository = gameRepository;
        _userGameRepository = userGameRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<Guid>> Handle(AcquireGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId);
        if (game is null)
            return Result<Guid>.Failure("Jogo não encontrado.");

        if (!game.IsActive)
            return Result<Guid>.Failure("Jogo não está disponível.");

        var alreadyOwns = await _userGameRepository.ExistsAsync(request.UserId, request.GameId);
        if (alreadyOwns)
            return Result<Guid>.Failure("Usuário já possui este jogo.");

        var userGame = UserGame.Create(request.UserId, request.GameId, game.FinalPrice);
        await _userGameRepository.AddAsync(userGame);
        await _userGameRepository.SaveChangesAsync();

        await _publishEndpoint.Publish(
            new OrderPlacedEvent(userGame.Id, request.UserId, request.GameId, game.FinalPrice),
            cancellationToken);

        return Result<Guid>.Success(userGame.Id);
    }
}
