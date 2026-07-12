using FCG.CatalogAPI.Domain.Entities;

namespace FCG.CatalogAPI.Tests.Domain;

public class GameTests
{
    [Fact]
    public void Create_ShouldCreateGame_WhenDataIsValid()
    {
        var game = Game.Create("The Witcher 3", "RPG de mundo aberto", 99.90m);

        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.Equal("The Witcher 3", game.Title);
        Assert.Equal(99.90m, game.Price);
        Assert.True(game.IsActive);
        Assert.Equal(0, game.DiscountPercentage);
    }

    [Fact]
    public void Create_ShouldThrow_WhenTitleIsEmpty()
    {
        var ex = Assert.Throws<ArgumentException>(
            () => Game.Create("", "Descrição", 50m));
        Assert.Contains("Título", ex.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenDescriptionIsEmpty()
    {
        var ex = Assert.Throws<ArgumentException>(
            () => Game.Create("Título", " ", 50m));
        Assert.Contains("Descrição", ex.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenPriceIsNegative()
    {
        var ex = Assert.Throws<ArgumentException>(
            () => Game.Create("Título", "Descrição", -1m));
        Assert.Contains("Preço", ex.Message);
    }

    [Fact]
    public void ApplyDiscount_ShouldSetDiscount_WhenPercentageIsValid()
    {
        var game = Game.Create("Jogo", "Descrição", 100m);

        game.ApplyDiscount(25m);

        Assert.Equal(25m, game.DiscountPercentage);
        Assert.Equal(75m, game.FinalPrice);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void ApplyDiscount_ShouldThrow_WhenPercentageIsInvalid(decimal percentage)
    {
        var game = Game.Create("Jogo", "Descrição", 100m);

        Assert.Throws<ArgumentException>(() => game.ApplyDiscount(percentage));
    }

    [Fact]
    public void Deactivate_ShouldDeactivateGame_WhenActive()
    {
        var game = Game.Create("Jogo", "Descrição", 100m);

        game.Deactivate();

        Assert.False(game.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldThrow_WhenAlreadyInactive()
    {
        var game = Game.Create("Jogo", "Descrição", 100m);
        game.Deactivate();

        Assert.Throws<InvalidOperationException>(() => game.Deactivate());
    }

    [Fact]
    public void FinalPrice_ShouldReturnFullPrice_WhenNoDiscount()
    {
        var game = Game.Create("Jogo", "Descrição", 59.90m);

        Assert.Equal(59.90m, game.FinalPrice);
    }
}
