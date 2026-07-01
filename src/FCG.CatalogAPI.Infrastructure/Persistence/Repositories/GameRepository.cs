using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FCG.CatalogAPI.Infrastructure.Persistence.Repositories;

public class GameRepository : IGameRepository
{
    private readonly AppDbContext _context;

    public GameRepository(AppDbContext context) => _context = context;

    public Task<Game?> GetByIdAsync(Guid id)
        => _context.Games.FirstOrDefaultAsync(g => g.Id == id);

    public async Task<IList<Game>> GetAllAsync()
        => await _context.Games.Where(g => g.IsActive).ToListAsync();

    public async Task AddAsync(Game game)
        => await _context.Games.AddAsync(game);

    public Task UpdateAsync(Game game)
    {
        _context.Games.Update(game);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}
