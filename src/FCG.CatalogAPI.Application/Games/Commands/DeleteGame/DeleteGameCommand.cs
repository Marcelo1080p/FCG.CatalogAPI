using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Commands.DeleteGame;

public record DeleteGameCommand(Guid Id) : IRequest<Result<bool>>;
