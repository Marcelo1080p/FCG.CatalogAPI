using FCG.CatalogAPI.Application.Games.Queries.SearchGames;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using NSubstitute;

namespace FCG.CatalogAPI.Tests.Application;

public class SearchGamesHandlerTests
{
    private readonly IGameSearchRepository _searchRepo = Substitute.For<IGameSearchRepository>();
    private readonly SearchGamesQueryHandler _handler;

    public SearchGamesHandlerTests()
        => _handler = new SearchGamesQueryHandler(_searchRepo);

    private static GameSearchHit Hit(string titulo, double score)
        => new(Guid.NewGuid(), titulo, "Descrição", 100m, 0m, 100m, score);

    [Fact]
    public async Task Handle_ShouldReturnResults_WhenTermMatches()
    {
        _searchRepo.SearchAsync("witcher", Arg.Any<int>(), Arg.Any<CancellationToken>())
                   .Returns(new List<GameSearchHit> { Hit("The Witcher 3", 8.5) });

        var result = await _handler.Handle(new SearchGamesQuery("witcher"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
        Assert.Equal("The Witcher 3", result.Value![0].Title);
    }

    [Fact]
    public async Task Handle_ShouldPreserveRelevanceOrder_FromSearchEngine()
    {
        _searchRepo.SearchAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                   .Returns(new List<GameSearchHit>
                   {
                       Hit("Mais relevante", 9.1),
                       Hit("Menos relevante", 2.4)
                   });

        var result = await _handler.Handle(new SearchGamesQuery("jogo"), CancellationToken.None);

        Assert.Equal("Mais relevante", result.Value![0].Title);
        Assert.True(result.Value[0].Score > result.Value[1].Score);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNothingMatches()
    {
        _searchRepo.SearchAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                   .Returns(new List<GameSearchHit>());

        var result = await _handler.Handle(new SearchGamesQuery("inexistente"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_ShouldFail_WhenTermIsEmpty(string termo)
    {
        var result = await _handler.Handle(new SearchGamesQuery(termo), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("termo", result.Error, StringComparison.OrdinalIgnoreCase);
        await _searchRepo.DidNotReceive().SearchAsync(
            Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldTrimTerm_BeforeSearching()
    {
        await _handler.Handle(new SearchGamesQuery("  witcher  "), CancellationToken.None);

        await _searchRepo.Received(1).SearchAsync(
            "witcher", Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(200, 50)]
    [InlineData(20, 20)]
    public async Task Handle_ShouldClampSize_WithinAllowedRange(int solicitado, int esperado)
    {
        await _handler.Handle(new SearchGamesQuery("jogo", solicitado), CancellationToken.None);

        await _searchRepo.Received(1).SearchAsync(
            Arg.Any<string>(), esperado, Arg.Any<CancellationToken>());
    }
}
