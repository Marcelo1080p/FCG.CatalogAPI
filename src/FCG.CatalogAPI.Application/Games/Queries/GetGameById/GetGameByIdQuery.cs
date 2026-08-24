using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Games.Dtos;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Queries.GetGameById;

public record GetGameByIdQuery(Guid Id) : IRequest<Result<GameDto>>;
