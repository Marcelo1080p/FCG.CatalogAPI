using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FCG.CatalogAPI.Domain.Entities;
using FCG.CatalogAPI.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FCG.CatalogAPI.Infrastructure.Search;

public class ElasticGameSearchRepository : IGameSearchRepository
{
    private readonly ElasticsearchClient _client;
    private readonly string _indexName;
    private readonly ILogger<ElasticGameSearchRepository> _logger;

    public ElasticGameSearchRepository(
        ElasticsearchClient client,
        ElasticsearchSettings settings,
        ILogger<ElasticGameSearchRepository> logger)
    {
        _client = client;
        _indexName = settings.IndexName;
        _logger = logger;
    }

    public async Task IndexAsync(Game game, CancellationToken cancellationToken = default)
    {
        try
        {
            var document = GameDocument.FromEntity(game);

            await _client.IndexAsync(
                document,
                i => i.Index(_indexName).Id(document.Id),
                cancellationToken);

            _logger.LogInformation("[Busca] Jogo indexado — {Title} ({GameId})", game.Title, game.Id);
        }
        catch (Exception ex)
        {
            // Uma falha na indexacao nao pode impedir a operacao de negocio:
            // o banco relacional continua sendo a fonte da verdade.
            _logger.LogWarning(ex, "[Busca] Falha ao indexar o jogo {GameId}", game.Id);
        }
    }

    public async Task RemoveAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.DeleteAsync(
                new DeleteRequest(_indexName, new Id(gameId.ToString())),
                cancellationToken);
            _logger.LogInformation("[Busca] Jogo removido do indice — {GameId}", gameId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Busca] Falha ao remover o jogo {GameId} do indice", gameId);
        }
    }

    public async Task<IList<GameSearchHit>> SearchAsync(
        string term, int size = 20, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(term))
            return new List<GameSearchHit>();

        try
        {
            var response = await _client.SearchAsync<GameDocument>(s => s
                .Index(_indexName)
                .Size(size)
                .Query(q => q
                    .Bool(b => b
                        // Apenas jogos ativos aparecem na busca.
                        .Filter(f => f.Term(t => t.Field(d => d.IsActive).Value(true)))
                        .Must(m => m
                            .MultiMatch(mm => mm
                                // O titulo pesa o triplo da descricao na relevancia.
                                .Fields(new[] { "title^3", "description" })
                                .Query(term)
                                // Tolerancia a erros de digitacao proporcional ao tamanho do termo.
                                .Fuzziness(new Fuzziness("AUTO"))
                                .PrefixLength(1))))),
                cancellationToken);

            if (!response.IsValidResponse)
            {
                _logger.LogWarning("[Busca] Consulta rejeitada pelo Elasticsearch — termo: {Term}", term);
                return new List<GameSearchHit>();
            }

            // Os resultados ja vem ordenados por relevancia (maior score primeiro).
            var hits = response.Hits
                .Where(h => h.Source is not null)
                .Select(h => h.Source!.ToSearchHit(h.Score ?? 0))
                .ToList();

            _logger.LogInformation(
                "[Busca] Termo '{Term}' retornou {Total} resultado(s)", term, hits.Count);

            return hits;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Busca] Falha ao pesquisar pelo termo {Term}", term);
            return new List<GameSearchHit>();
        }
    }

    /// <summary>
    /// Cria o indice com o mapeamento dos campos, caso ainda nao exista.
    /// </summary>
    public async Task EnsureIndexAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _client.Indices.ExistsAsync(_indexName, cancellationToken);
            if (exists.Exists)
                return;

            await _client.Indices.CreateAsync<GameDocument>(_indexName, c => c
                .Mappings(m => m
                    .Properties(p => p
                        .Keyword(k => k.Id)
                        .Text(t => t.Title)
                        .Text(t => t.Description)
                        .Boolean(b => b.IsActive))),
                cancellationToken);

            _logger.LogInformation("[Busca] Indice '{Index}' criado.", _indexName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Busca] Falha ao preparar o indice '{Index}'", _indexName);
        }
    }
}
