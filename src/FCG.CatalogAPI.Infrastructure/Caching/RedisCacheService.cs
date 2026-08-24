using FCG.CatalogAPI.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FCG.CatalogAPI.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = await _cache.GetStringAsync(key, cancellationToken);
            if (string.IsNullOrEmpty(payload))
            {
                _logger.LogInformation("[Cache] MISS — {Key}", key);
                return default;
            }

            _logger.LogInformation("[Cache] HIT — {Key}", key);
            return JsonSerializer.Deserialize<T>(payload);
        }
        catch (Exception ex)
        {
            // Uma falha no cache não pode derrubar a requisição: segue para a origem.
            _logger.LogWarning(ex, "[Cache] Falha ao ler a chave {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
            };

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options, cancellationToken);
            _logger.LogInformation("[Cache] SET — {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Cache] Falha ao gravar a chave {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _logger.LogInformation("[Cache] INVALIDADO — {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Cache] Falha ao remover a chave {Key}", key);
        }
    }
}
