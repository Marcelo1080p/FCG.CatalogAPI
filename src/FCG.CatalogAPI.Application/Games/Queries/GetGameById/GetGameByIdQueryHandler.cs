using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Queries.GetGameById;

public class GetGameByIdQueryHandler : IRequestHandler<GetGameByIdQuery, Result<Game>>
{
    private readonly IGameRepository _gameRepository;

    public GetGameByIdQueryHandler(IGameRepository gameRepository)
        => _gameRepository = gameRepository;

    public async Task<Result<Game>> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.Id);
        if (game is null)
            return Result<Game>.Failure("Jogo não encontrado.");
        return Result<Game>.Success(game);
    }
}
