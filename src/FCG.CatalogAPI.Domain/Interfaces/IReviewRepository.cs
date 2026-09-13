using FCG.CatalogAPI.Domain.Entities;

namespace FCG.CatalogAPI.Domain.Interfaces;

public interface IReviewRepository
{
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    Task<IList<Review>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid gameId, Guid userId, CancellationToken cancellationToken = default);
    Task<IDictionary<Guid, GameRating>> GetRatingsAsync(IEnumerable<Guid> gameIds, CancellationToken cancellationToken = default);
}
