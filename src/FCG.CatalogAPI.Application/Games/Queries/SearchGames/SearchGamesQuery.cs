using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Games.Dtos;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Queries.SearchGames;

public record SearchGamesQuery(string Term, int Size = 20)
    : IRequest<Result<IList<GameSearchResultDto>>>;
