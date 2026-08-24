using FCG.CatalogAPI.Application.Games;
using FCG.CatalogAPI.Application.Games.Commands.ApplyDiscount;
using FCG.CatalogAPI.Application.Interfaces;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using NSubstitute;

namespace FCG.CatalogAPI.Tests.Application;

public class ApplyDiscountHandlerTests
{
    private readonly IGameRepository _gameRepo = Substitute.For<IGameRepository>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly ApplyDiscountCommandHandler _handler;

    public ApplyDiscountHandlerTests()
        => _handler = new ApplyDiscountCommandHandler(_gameRepo, _cache);

    [Fact]
    public async Task Handle_ShouldApplyDiscount_WhenGameExists()
    {
        var game = Game.Create("Jogo", "Descrição", 100m);
        _gameRepo.GetByIdAsync(game.Id).Returns(game);

        var result = await _handler.Handle(
            new ApplyDiscountCommand(game.Id, 30m), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(30m, game.DiscountPercentage);
        Assert.Equal(70m, game.FinalPrice);
        await _gameRepo.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ShouldInvalidateCache_WhenDiscountIsApplied()
    {
        var game = Game.Create("Jogo", "Descrição", 100m);
        _gameRepo.GetByIdAsync(game.Id).Returns(game);

        await _handler.Handle(
            new ApplyDiscountCommand(game.Id, 30m), CancellationToken.None);

        await _cache.Received(1).RemoveAsync(CacheKeys.AllGames, Arg.Any<CancellationToken>());
        await _cache.Received(1).RemoveAsync(CacheKeys.Game(game.Id), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenGameNotFound()
    {
        _gameRepo.GetByIdAsync(Arg.Any<Guid>()).Returns((Game?)null);

        var result = await _handler.Handle(
            new ApplyDiscountCommand(Guid.NewGuid(), 30m), CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPercentageIsInvalid()
    {
        var game = Game.Create("Jogo", "Descrição", 100m);
        _gameRepo.GetByIdAsync(game.Id).Returns(game);

        var result = await _handler.Handle(
            new ApplyDiscountCommand(game.Id, 150m), CancellationToken.None);

        Assert.False(result.IsSuccess);
        await _gameRepo.DidNotReceive().SaveChangesAsync();
        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
