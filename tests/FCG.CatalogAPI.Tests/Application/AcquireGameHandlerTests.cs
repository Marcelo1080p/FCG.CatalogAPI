using FCG.CatalogAPI.Application.Games.Commands.AcquireGame;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using FCG.Contracts.Events;
using MassTransit;
using NSubstitute;

namespace FCG.CatalogAPI.Tests.Application;

public class AcquireGameHandlerTests
{
    private readonly IGameRepository _gameRepo = Substitute.For<IGameRepository>();
    private readonly IUserGameRepository _userGameRepo = Substitute.For<IUserGameRepository>();
    private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();
    private readonly AcquireGameCommandHandler _handler;

    public AcquireGameHandlerTests()
        => _handler = new AcquireGameCommandHandler(_gameRepo, _userGameRepo, _publishEndpoint);

    [Fact]
    public async Task Handle_ShouldAcquireGameAndPublishEvent_WhenValid()
    {
        var game = Game.Create("Celeste", "Plataforma", 36.99m);
        var userId = Guid.NewGuid();
        _gameRepo.GetByIdAsync(game.Id).Returns(game);
        _userGameRepo.ExistsAsync(userId, game.Id).Returns(false);

        var result = await _handler.Handle(
            new AcquireGameCommand(userId, game.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _userGameRepo.Received(1).AddAsync(Arg.Any<UserGame>());
        await _userGameRepo.Received(1).SaveChangesAsync();
        await _publishEndpoint.Received(1).Publish(
            Arg.Is<OrderPlacedEvent>(e => e.UserId == userId && e.GameId == game.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenGameNotFound()
    {
        _gameRepo.GetByIdAsync(Arg.Any<Guid>()).Returns((Game?)null);

        var result = await _handler.Handle(
            new AcquireGameCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("não encontrado", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenGameIsInactive()
    {
        var game = Game.Create("Jogo", "Descrição", 10m);
        game.Deactivate();
        _gameRepo.GetByIdAsync(game.Id).Returns(game);

        var result = await _handler.Handle(
            new AcquireGameCommand(Guid.NewGuid(), game.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("não está disponível", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserAlreadyOwnsGame()
    {
        var game = Game.Create("Jogo", "Descrição", 10m);
        var userId = Guid.NewGuid();
        _gameRepo.GetByIdAsync(game.Id).Returns(game);
        _userGameRepo.ExistsAsync(userId, game.Id).Returns(true);

        var result = await _handler.Handle(
            new AcquireGameCommand(userId, game.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("já possui", result.Error);
        await _publishEndpoint.DidNotReceive().Publish(
            Arg.Any<OrderPlacedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldUseFinalPriceWithDiscount_WhenAcquiring()
    {
        var game = Game.Create("Jogo", "Descrição", 100m);
        game.ApplyDiscount(50m);
        var userId = Guid.NewGuid();
        _gameRepo.GetByIdAsync(game.Id).Returns(game);
        _userGameRepo.ExistsAsync(userId, game.Id).Returns(false);

        await _handler.Handle(new AcquireGameCommand(userId, game.Id), CancellationToken.None);

        await _publishEndpoint.Received(1).Publish(
            Arg.Is<OrderPlacedEvent>(e => e.Amount == 50m),
            Arg.Any<CancellationToken>());
    }
}
