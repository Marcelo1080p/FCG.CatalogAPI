using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using MediatR;

namespace FCG.CatalogAPI.Application.Games.Commands.ApplyDiscount;

public record ApplyDiscountCommand(Guid GameId, decimal Percentage) : IRequest<Result<bool>>;
