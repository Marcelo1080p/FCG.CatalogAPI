namespace FCG.CatalogAPI.Domain.Entities;

/// <summary>
/// Nota agregada de um jogo, calculada a partir das avaliações.
/// </summary>
public record GameRating(Guid GameId, double Average, int Count);
