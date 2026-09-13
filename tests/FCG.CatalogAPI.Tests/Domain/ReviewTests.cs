using FCG.CatalogAPI.Domain.Entities;

namespace FCG.CatalogAPI.Tests.Domain;

public class ReviewTests
{
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public void Create_ShouldCreateReview_WhenDataIsValid()
    {
        var review = Review.Create(GameId, UserId, 5, "Excelente jogo");

        Assert.NotEqual(Guid.Empty, review.Id);
        Assert.Equal(GameId, review.GameId);
        Assert.Equal(UserId, review.UserId);
        Assert.Equal(5, review.Rating);
        Assert.Equal("Excelente jogo", review.Comment);
        Assert.Empty(review.Tags);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Create_ShouldThrow_WhenRatingIsOutOfRange(int rating)
    {
        var ex = Assert.Throws<ArgumentException>(
            () => Review.Create(GameId, UserId, rating, "Comentário"));
        Assert.Contains("Nota", ex.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenGameIsEmpty()
    {
        Assert.Throws<ArgumentException>(
            () => Review.Create(Guid.Empty, UserId, 5, "Comentário"));
    }

    [Fact]
    public void Create_ShouldThrow_WhenUserIsEmpty()
    {
        Assert.Throws<ArgumentException>(
            () => Review.Create(GameId, Guid.Empty, 5, "Comentário"));
    }

    [Fact]
    public void Create_ShouldAcceptReview_WhenCommentIsNull()
    {
        var review = Review.Create(GameId, UserId, 4, null);

        Assert.Equal(string.Empty, review.Comment);
    }

    [Fact]
    public void Create_ShouldThrow_WhenCommentExceedsLimit()
    {
        var ex = Assert.Throws<ArgumentException>(
            () => Review.Create(GameId, UserId, 4, new string('a', 1001)));
        Assert.Contains("Comentário", ex.Message);
    }

    [Fact]
    public void Create_ShouldNormalizeTags_RemovingDuplicatesAndCasing()
    {
        var review = Review.Create(GameId, UserId, 5, "Ótimo", new[] { "RPG", "rpg", " Indie ", "" });

        Assert.Equal(2, review.Tags.Count);
        Assert.Contains("rpg", review.Tags);
        Assert.Contains("indie", review.Tags);
    }

    [Fact]
    public void Create_ShouldThrow_WhenTagsExceedLimit()
    {
        var tags = new[] { "a", "b", "c", "d", "e", "f" };

        var ex = Assert.Throws<ArgumentException>(
            () => Review.Create(GameId, UserId, 5, "Ótimo", tags));
        Assert.Contains("tags", ex.Message);
    }

    [Fact]
    public void Restore_ShouldRebuildReview_WithOriginalValues()
    {
        var id = Guid.NewGuid();
        var createdAt = new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        var review = Review.Restore(id, GameId, UserId, 3, "Comentário", new[] { "casual" }, createdAt);

        Assert.Equal(id, review.Id);
        Assert.Equal(3, review.Rating);
        Assert.Equal(createdAt, review.CreatedAt);
        Assert.Single(review.Tags);
    }
}
