using MediatR;

namespace FCG.CatalogAPI.Application.Games.Commands.CreateGame;

public record CreateGameCommand(string Title, string Description, decimal Price)
    : IRequest<Result<Guid>>;

public class Result<T>
{
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public bool IsSuccess => Error is null;

    public static Result<T> Success(T value) => new() { Value = value };
    public static Result<T> Failure(string error) => new() { Error = error };
}
