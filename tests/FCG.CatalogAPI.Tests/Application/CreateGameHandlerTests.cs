using FCG.CatalogAPI.Application.Games;
using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Interfaces;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using NSubstitute;

namespace FCG.CatalogAPI.Tests.Application;

public class CreateGameHandlerTests
{
    private readonly IGameRepository _gameRepo = Substitute.For<IGameRepository>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly CreateGameCommandHandler _handler;

    public CreateGameHandlerTests()
        => _handler = new CreateGameCommandHandler(_gameRepo, _cache);

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
    public async Task Handle_ShouldInvalidateCache_WhenGameIsCreated()
    {
        var cmd = new CreateGameCommand("Hollow Knight", "Metroidvania", 46.99m);

        await _handler.Handle(cmd, CancellationToken.None);

        await _cache.Received(1).RemoveAsync(CacheKeys.AllGames, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenTitleIsInvalid()
    {
        var cmd = new CreateGameCommand("", "Descrição", 10m);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        await _gameRepo.DidNotReceive().AddAsync(Arg.Any<Game>());
        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
