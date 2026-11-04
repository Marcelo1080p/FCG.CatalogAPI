namespace FCG.CatalogAPI.Domain.Entities;

/// <summary>
/// Resultado de uma busca no catalogo, acompanhado da pontuacao de
/// relevancia atribuida pelo motor de busca.
/// </summary>
public record GameSearchHit(
    Guid GameId,
    string Title,
    string Description,
    decimal Price,
    decimal DiscountPercentage,
    decimal FinalPrice,
    double Score);
