using FCG.CatalogAPI.Application.Games;
using FCG.CatalogAPI.Application.Interfaces;
using FCG.CatalogAPI.Application.Reviews.Commands.CreateReview;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using NSubstitute;

namespace FCG.CatalogAPI.Tests.Application;

public class CreateReviewHandlerTests
{
    private readonly IGameRepository _gameRepo = Substitute.For<IGameRepository>();
    private readonly IReviewRepository _reviewRepo = Substitute.For<IReviewRepository>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly CreateReviewCommandHandler _handler;

    private readonly Game _game = Game.Create("Hollow Knight", "Metroidvania", 46.99m);

    public CreateReviewHandlerTests()
    {
        _gameRepo.GetByIdAsync(_game.Id).Returns(_game);
        _handler = new CreateReviewCommandHandler(_gameRepo, _reviewRepo, _cache);
    }

    [Fact]
    public async Task Handle_ShouldPersistReview_WhenDataIsValid()
    {
        var userId = Guid.NewGuid();

        var result = await _handler.Handle(
            new CreateReviewCommand(_game.Id, userId, 5, "Muito bom", new[] { "indie" }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _reviewRepo.Received(1).AddAsync(
            Arg.Is<Review>(r => r.GameId == _game.Id && r.UserId == userId && r.Rating == 5),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldInvalidateCatalogCache_WhenReviewIsCreated()
    {
        await _handler.Handle(
            new CreateReviewCommand(_game.Id, Guid.NewGuid(), 4, "Bom", null),
            CancellationToken.None);

        await _cache.Received(1).RemoveAsync(CacheKeys.AllGames, Arg.Any<CancellationToken>());
        await _cache.Received(1).RemoveAsync(CacheKeys.Game(_game.Id), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenGameNotFound()
    {
        _gameRepo.GetByIdAsync(Arg.Any<Guid>()).Returns((Game?)null);

        var result = await _handler.Handle(
            new CreateReviewCommand(Guid.NewGuid(), Guid.NewGuid(), 5, "Bom", null),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("não encontrado", result.Error);
        await _reviewRepo.DidNotReceive().AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserAlreadyReviewedGame()
    {
        var userId = Guid.NewGuid();
        _reviewRepo.ExistsAsync(_game.Id, userId, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _handler.Handle(
            new CreateReviewCommand(_game.Id, userId, 5, "De novo", null),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("já avaliou", result.Error);
        await _reviewRepo.DidNotReceive().AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRatingIsInvalid()
    {
        var result = await _handler.Handle(
            new CreateReviewCommand(_game.Id, Guid.NewGuid(), 9, "Nota inválida", null),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        await _reviewRepo.DidNotReceive().AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());
        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
