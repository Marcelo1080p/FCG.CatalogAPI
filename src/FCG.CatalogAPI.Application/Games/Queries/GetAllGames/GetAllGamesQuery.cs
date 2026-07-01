using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Domain.Entities;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Queries.GetAllGames;

public record GetAllGamesQuery : IRequest<Result<IList<Game>>>;
