using System.Security.Cryptography;
using System.Text.Json;
using StackExchange.Redis;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Cache;

namespace Yemekhane.Backend.Infrastructure.Auth;

public sealed class RedisSessionService(
    IConnectionMultiplexer redis) : ISessionService
{
    private static readonly TimeSpan SessionDuration = TimeSpan.FromMinutes(30);

    private IDatabase Database => redis.GetDatabase();

    public async Task<string> CreateAsync(SessionUser user, CancellationToken cancellationToken = default)
    {
        var token = Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(32));

        await Database.StringSetAsync(CacheKeys.Session(token), JsonSerializer.Serialize(user), SessionDuration);

        return token;
    }

    public async Task<SessionUser?> GetAsync(string token, CancellationToken cancellationToken = default)
    {
        var value = await Database.StringGetSetExpiryAsync(CacheKeys.Session(token), SessionDuration);

        return value.IsNullOrEmpty
            ? null
            : JsonSerializer.Deserialize<SessionUser>(value.ToString());
    }

    public Task RemoveAsync(string token, CancellationToken cancellationToken = default) =>
        Database.KeyDeleteAsync(CacheKeys.Session(token));
}
