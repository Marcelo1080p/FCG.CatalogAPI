using FCG.CatalogAPI.Application.Games;
using FCG.CatalogAPI.Application.Games.Dtos;
using FCG.CatalogAPI.Application.Games.Queries.GetAllGames;
using FCG.CatalogAPI.Application.Interfaces;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using NSubstitute;

namespace FCG.CatalogAPI.Tests.Application;

public class GetAllGamesHandlerTests
{
    private readonly IGameRepository _gameRepo = Substitute.For<IGameRepository>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly GetAllGamesQueryHandler _handler;

    public GetAllGamesHandlerTests()
    {
        // Por padrão o cache está vazio; os testes de hit sobrescrevem este retorno.
        _cache.GetAsync<IList<GameDto>>(CacheKeys.AllGames, Arg.Any<CancellationToken>())
              .Returns((IList<GameDto>?)null);

        _handler = new GetAllGamesQueryHandler(_gameRepo, _cache);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllGames_WhenCacheIsEmpty()
    {
        var games = new List<Game>
        {
            Game.Create("Jogo A", "Descrição A", 10m),
            Game.Create("Jogo B", "Descrição B", 20m)
        };
        _gameRepo.GetAllAsync().Returns(games);

        var result = await _handler.Handle(new GetAllGamesQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count);
    }

    [Fact]
    public async Task Handle_ShouldStoreResultInCache_WhenCacheIsEmpty()
    {
        _gameRepo.GetAllAsync().Returns(new List<Game> { Game.Create("Jogo", "Descrição", 10m) });

        await _handler.Handle(new GetAllGamesQuery(), CancellationToken.None);

        await _cache.Received(1).SetAsync(
            CacheKeys.AllGames,
            Arg.Any<IList<GameDto>>(),
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnCachedResult_WithoutQueryingRepository()
    {
        var cached = new List<GameDto>
        {
            GameDto.FromEntity(Game.Create("Jogo em cache", "Descrição", 30m))
        };
        _cache.GetAsync<IList<GameDto>>(CacheKeys.AllGames, Arg.Any<CancellationToken>())
              .Returns(cached);

        var result = await _handler.Handle(new GetAllGamesQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
        Assert.Equal("Jogo em cache", result.Value![0].Title);
        await _gameRepo.DidNotReceive().GetAllAsync();
    }

    [Fact]
    public async Task Handle_ShouldExposeFinalPrice_WhenGameHasDiscount()
    {
        var game = Game.Create("Jogo", "Descrição", 200m);
        game.ApplyDiscount(50m);
        _gameRepo.GetAllAsync().Returns(new List<Game> { game });

        var result = await _handler.Handle(new GetAllGamesQuery(), CancellationToken.None);

        Assert.Equal(200m, result.Value![0].Price);
        Assert.Equal(100m, result.Value![0].FinalPrice);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoGamesExist()
    {
        _gameRepo.GetAllAsync().Returns(new List<Game>());

        var result = await _handler.Handle(new GetAllGamesQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }
}
