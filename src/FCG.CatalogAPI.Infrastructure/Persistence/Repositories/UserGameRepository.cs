using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FCG.CatalogAPI.Infrastructure.Persistence.Repositories;

public class UserGameRepository : IUserGameRepository
{
    private readonly AppDbContext _context;

    public UserGameRepository(AppDbContext context) => _context = context;

    public Task<bool> ExistsAsync(Guid userId, Guid gameId)
        => _context.UserGames.AnyAsync(ug => ug.UserId == userId && ug.GameId == gameId);

    public async Task<IList<UserGame>> GetByUserIdAsync(Guid userId)
        => await _context.UserGames
            .Include(ug => ug.Game)
            .Where(ug => ug.UserId == userId)
            .ToListAsync();

    public async Task AddAsync(UserGame userGame)
        => await _context.UserGames.AddAsync(userGame);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}
