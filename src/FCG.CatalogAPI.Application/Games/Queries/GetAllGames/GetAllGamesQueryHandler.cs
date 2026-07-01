using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Queries.GetAllGames;

public class GetAllGamesQueryHandler : IRequestHandler<GetAllGamesQuery, Result<IList<Game>>>
{
    private readonly IGameRepository _gameRepository;

    public GetAllGamesQueryHandler(IGameRepository gameRepository)
        => _gameRepository = gameRepository;

    public async Task<Result<IList<Game>>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
    {
        var games = await _gameRepository.GetAllAsync();
        return Result<IList<Game>>.Success(games);
    }
}
