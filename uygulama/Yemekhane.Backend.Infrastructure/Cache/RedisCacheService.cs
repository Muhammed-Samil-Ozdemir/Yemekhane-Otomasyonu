using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using StackExchange.Redis;
using Yemekhane.Backend.Application.Cache;

namespace Yemekhane.Backend.Infrastructure.Cache;

public sealed class RedisCacheService(
    IConnectionMultiplexer redis) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    private IDatabase Database => redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await Database.StringGetAsync(key);

        return value.IsNullOrEmpty
            ? default
            : JsonSerializer.Deserialize<T>(value.ToString(), JsonOptions);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);

        return expiration is null
            ? Database.StringSetAsync(key, json)
            : Database.StringSetAsync(key, json, expiration.Value);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        Database.KeyDeleteAsync(key);

    public async Task<long> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        long removedCount = 0;

        foreach (var endPoint in redis.GetEndPoints())
        {
            var server = redis.GetServer(endPoint);

            await foreach (var key in server.KeysAsync(pattern: pattern).WithCancellation(cancellationToken))
            {
                if (await Database.KeyDeleteAsync(key))
                    removedCount++;
            }
        }

        return removedCount;
    }
}
