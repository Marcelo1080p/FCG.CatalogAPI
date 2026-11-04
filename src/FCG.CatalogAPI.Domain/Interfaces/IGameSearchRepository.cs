using FCG.CatalogAPI.Domain.Entities;

namespace FCG.CatalogAPI.Domain.Interfaces;

/// <summary>
/// Busca textual do catalogo. O banco relacional continua sendo a fonte da
/// verdade: este indice e uma copia otimizada apenas para leitura de busca.
/// </summary>
public interface IGameSearchRepository
{
    /// <summary>Cria ou atualiza o jogo no indice.</summary>
    Task IndexAsync(Game game, CancellationToken cancellationToken = default);

    /// <summary>Remove o jogo do indice.</summary>
    Task RemoveAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca por termo, tolerando erros de digitacao e ordenando por relevancia.
    /// </summary>
    Task<IList<GameSearchHit>> SearchAsync(string term, int size = 20, CancellationToken cancellationToken = default);
}
