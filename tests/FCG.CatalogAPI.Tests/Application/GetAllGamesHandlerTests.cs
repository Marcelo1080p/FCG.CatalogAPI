using FCG.CatalogAPI.Application.Games.Queries.GetAllGames;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using NSubstitute;

namespace FCG.CatalogAPI.Tests.Application;

public class GetAllGamesHandlerTests
{
    private readonly IGameRepository _gameRepo = Substitute.For<IGameRepository>();
    private readonly GetAllGamesQueryHandler _handler;

    public GetAllGamesHandlerTests()
        => _handler = new GetAllGamesQueryHandler(_gameRepo);

    [Fact]
    public async Task Handle_ShouldReturnAllGames()
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
    public async Task Handle_ShouldReturnEmptyList_WhenNoGamesExist()
    {
        _gameRepo.GetAllAsync().Returns(new List<Game>());

        var result = await _handler.Handle(new GetAllGamesQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }
}
