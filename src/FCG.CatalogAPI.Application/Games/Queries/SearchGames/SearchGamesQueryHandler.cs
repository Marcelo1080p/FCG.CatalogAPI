using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Games.Dtos;
using FCG.CatalogAPI.Domain.Interfaces;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Queries.SearchGames;

public class SearchGamesQueryHandler
    : IRequestHandler<SearchGamesQuery, Result<IList<GameSearchResultDto>>>
{
    private const int MaxSize = 50;

    private readonly IGameSearchRepository _searchRepository;

    public SearchGamesQueryHandler(IGameSearchRepository searchRepository)
        => _searchRepository = searchRepository;

    public async Task<Result<IList<GameSearchResultDto>>> Handle(
        SearchGamesQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Term))
            return Result<IList<GameSearchResultDto>>.Failure("Informe um termo para a busca.");

        var size = Math.Clamp(request.Size, 1, MaxSize);

        var hits = await _searchRepository.SearchAsync(
            request.Term.Trim(), size, cancellationToken);

        IList<GameSearchResultDto> resultados = hits
            .Select(GameSearchResultDto.FromHit)
            .ToList();

        return Result<IList<GameSearchResultDto>>.Success(resultados);
    }
}
