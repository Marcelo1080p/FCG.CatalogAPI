using FCG.CatalogAPI.Domain.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace FCG.CatalogAPI.Infrastructure.Persistence.Mongo;

public class ReviewDocument
{
    [BsonId]
    public string Id { get; set; } = string.Empty;

    [BsonElement("gameId")]
    public string GameId { get; set; } = string.Empty;

    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("rating")]
    public int Rating { get; set; }

    [BsonElement("comment")]
    public string Comment { get; set; } = string.Empty;

    [BsonElement("tags")]
    public string[] Tags { get; set; } = Array.Empty<string>();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    public static ReviewDocument FromEntity(Review review) => new()
    {
        Id = review.Id.ToString(),
        GameId = review.GameId.ToString(),
        UserId = review.UserId.ToString(),
        Rating = review.Rating,
        Comment = review.Comment,
        Tags = review.Tags.ToArray(),
        CreatedAt = review.CreatedAt
    };

    public Review ToEntity() => Review.Restore(
        Guid.Parse(Id),
        Guid.Parse(GameId),
        Guid.Parse(UserId),
        Rating,
        Comment,
        Tags,
        CreatedAt);
}
