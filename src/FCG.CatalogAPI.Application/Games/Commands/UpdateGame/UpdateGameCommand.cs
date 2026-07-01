using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Commands.UpdateGame;

public record UpdateGameCommand(Guid Id, string Title, string Description, decimal Price)
    : IRequest<Result<bool>>;
