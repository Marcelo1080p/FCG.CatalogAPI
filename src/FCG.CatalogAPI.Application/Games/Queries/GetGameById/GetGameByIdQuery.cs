using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Domain.Entities;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Queries.GetGameById;

public record GetGameByIdQuery(Guid Id) : IRequest<Result<Game>>;
