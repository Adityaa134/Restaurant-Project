using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<CacheService> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CacheService(IDistributedCache distributedCache, ILogger<CacheService> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                string? cached = await _distributedCache.GetStringAsync(key);
                if (string.IsNullOrEmpty(cached))
                    return default;

                return JsonSerializer.Deserialize<T>(cached, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache GET failed for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? baseExpiry = null)
        {
            if (value == null) return;

            try
            {
                var baseTtl = baseExpiry ?? TimeSpan.FromMinutes(10);
                int jitterSeconds = Random.Shared.Next(-30, 31);
                var finalTtl = baseTtl.Add(TimeSpan.FromSeconds(jitterSeconds));
                if (finalTtl <= TimeSpan.FromSeconds(5))
                {
                    finalTtl = baseTtl;
                }

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = finalTtl
                };

                string json = JsonSerializer.Serialize(value, JsonOptions);
                await _distributedCache.SetStringAsync(key, json, options);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache SET failed for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _distributedCache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache REMOVE failed for key: {Key}", key);
            }
        }
    }
}
