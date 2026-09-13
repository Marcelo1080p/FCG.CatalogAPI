using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using MongoDB.Driver;

namespace FCG.CatalogAPI.Infrastructure.Persistence.Mongo;

public class MongoReviewRepository : IReviewRepository
{
    public const string CollectionName = "reviews";

    private readonly IMongoCollection<ReviewDocument> _collection;

    public MongoReviewRepository(IMongoDatabase database)
        => _collection = database.GetCollection<ReviewDocument>(CollectionName);

    public Task AddAsync(Review review, CancellationToken cancellationToken = default)
        => _collection.InsertOneAsync(
            ReviewDocument.FromEntity(review), cancellationToken: cancellationToken);

    public async Task<IList<Review>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var id = gameId.ToString();

        var documents = await _collection
            .Find(d => d.GameId == id)
            .SortByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);

        return documents.Select(d => d.ToEntity()).ToList();
    }

    public async Task<bool> ExistsAsync(Guid gameId, Guid userId, CancellationToken cancellationToken = default)
    {
        var game = gameId.ToString();
        var user = userId.ToString();

        var count = await _collection.CountDocumentsAsync(
            d => d.GameId == game && d.UserId == user,
            cancellationToken: cancellationToken);

        return count > 0;
    }

    public async Task<IDictionary<Guid, GameRating>> GetRatingsAsync(
        IEnumerable<Guid> gameIds, CancellationToken cancellationToken = default)
    {
        var ids = gameIds.Select(g => g.ToString()).ToList();
        if (ids.Count == 0)
            return new Dictionary<Guid, GameRating>();

        // Agregacao no proprio MongoDB: media e contagem por jogo em uma unica consulta.
        var aggregated = await _collection.Aggregate()
            .Match(d => ids.Contains(d.GameId))
            .Group(d => d.GameId, g => new
            {
                GameId = g.Key,
                Average = g.Average(x => x.Rating),
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        return aggregated.ToDictionary(
            r => Guid.Parse(r.GameId),
            r => new GameRating(Guid.Parse(r.GameId), Math.Round(r.Average, 2), r.Count));
    }
}
