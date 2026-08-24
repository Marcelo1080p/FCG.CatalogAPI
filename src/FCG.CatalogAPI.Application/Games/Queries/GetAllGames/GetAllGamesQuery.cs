using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Games.Dtos;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Queries.GetAllGames;

public record GetAllGamesQuery : IRequest<Result<IList<GameDto>>>;
