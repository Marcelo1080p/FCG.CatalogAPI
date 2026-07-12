using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using NSubstitute;

namespace FCG.CatalogAPI.Tests.Application;

public class CreateGameHandlerTests
{
    private readonly IGameRepository _gameRepo = Substitute.For<IGameRepository>();
    private readonly CreateGameCommandHandler _handler;

    public CreateGameHandlerTests()
        => _handler = new CreateGameCommandHandler(_gameRepo);

    [Fact]
    public async Task Handle_ShouldCreateGame_WhenDataIsValid()
    {
        var cmd = new CreateGameCommand("Hollow Knight", "Metroidvania", 46.99m);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        await _gameRepo.Received(1).AddAsync(Arg.Any<Game>());
        await _gameRepo.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenTitleIsInvalid()
    {
        var cmd = new CreateGameCommand("", "Descrição", 10m);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        await _gameRepo.DidNotReceive().AddAsync(Arg.Any<Game>());
    }
}
