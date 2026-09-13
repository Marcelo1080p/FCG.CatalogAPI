using FCG.CatalogAPI.Application.Reviews.Queries.GetGameReviews;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using NSubstitute;

namespace FCG.CatalogAPI.Tests.Application;

public class GetGameReviewsHandlerTests
{
    private readonly IGameRepository _gameRepo = Substitute.For<IGameRepository>();
    private readonly IReviewRepository _reviewRepo = Substitute.For<IReviewRepository>();
    private readonly GetGameReviewsQueryHandler _handler;

    private readonly Game _game = Game.Create("Celeste", "Plataforma", 36.99m);

    public GetGameReviewsHandlerTests()
    {
        _gameRepo.GetByIdAsync(_game.Id).Returns(_game);
        _handler = new GetGameReviewsQueryHandler(_gameRepo, _reviewRepo);
    }

    [Fact]
    public async Task Handle_ShouldReturnReviews_WhenGameExists()
    {
        var reviews = new List<Review>
        {
            Review.Create(_game.Id, Guid.NewGuid(), 5, "Perfeito"),
            Review.Create(_game.Id, Guid.NewGuid(), 4, "Muito bom")
        };
        _reviewRepo.GetByGameIdAsync(_game.Id, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(
            new GetGameReviewsQuery(_game.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count);
        Assert.Equal("Perfeito", result.Value[0].Comment);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenGameHasNoReviews()
    {
        _reviewRepo.GetByGameIdAsync(_game.Id, Arg.Any<CancellationToken>())
                   .Returns(new List<Review>());

        var result = await _handler.Handle(
            new GetGameReviewsQuery(_game.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenGameNotFound()
    {
        _gameRepo.GetByIdAsync(Arg.Any<Guid>()).Returns((Game?)null);

        var result = await _handler.Handle(
            new GetGameReviewsQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("não encontrado", result.Error);
    }
}
