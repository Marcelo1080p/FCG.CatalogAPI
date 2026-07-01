using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Commands.AcquireGame;

public record AcquireGameCommand(Guid UserId, Guid GameId) : IRequest<Result<Guid>>;
