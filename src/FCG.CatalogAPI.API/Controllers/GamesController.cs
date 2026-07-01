using FCG.CatalogAPI.Application.Games.Commands.AcquireGame;
using FCG.CatalogAPI.Application.Games.Commands.ApplyDiscount;
using FCG.CatalogAPI.Application.Games.Commands.CreateGame;
using FCG.CatalogAPI.Application.Games.Commands.DeleteGame;
using FCG.CatalogAPI.Application.Games.Commands.UpdateGame;
using FCG.CatalogAPI.Application.Games.Queries.GetAllGames;
using FCG.CatalogAPI.Application.Games.Queries.GetGameById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FCG.CatalogAPI.API.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly IMediator _mediator;

    public GamesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllGamesQuery());
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetGameByIdQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateGameRequest req)
    {
        var result = await _mediator.Send(new CreateGameCommand(req.Title, req.Description, req.Price));
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGameRequest req)
    {
        var result = await _mediator.Send(new UpdateGameCommand(id, req.Title, req.Description, req.Price));
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteGameCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/discount")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApplyDiscount(Guid id, [FromBody] ApplyDiscountRequest req)
    {
        var result = await _mediator.Send(new ApplyDiscountCommand(id, req.Percentage));
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/acquire")]
    [Authorize]
    public async Task<IActionResult> Acquire(Guid id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new AcquireGameCommand(userId, id));
        return result.IsSuccess ? Ok(new { orderId = result.Value }) : BadRequest(result.Error);
    }
}

public record CreateGameRequest(string Title, string Description, decimal Price);
public record UpdateGameRequest(string Title, string Description, decimal Price);
public record ApplyDiscountRequest(decimal Percentage);
