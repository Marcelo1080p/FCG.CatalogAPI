using FCG.CatalogAPI.Domain.Entities;

namespace FCG.CatalogAPI.Domain.Interfaces;

public interface IUserGameRepository
{
    Task<bool> ExistsAsync(Guid userId, Guid gameId);
    Task<IList<UserGame>> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserGame userGame);
    Task SaveChangesAsync();
}
