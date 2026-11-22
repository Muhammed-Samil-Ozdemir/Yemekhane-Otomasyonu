using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;
using Yemekhane.Backend.Application.Auth;

namespace Yemekhane.Backend.Infrastructure.Auth;

public sealed class MemorySessionService(
    IMemoryCache memoryCache) : ISessionService
{
    private static readonly TimeSpan SessionDuration = TimeSpan.FromMinutes(30);

    public Task<string> CreateAsync(SessionUser user, CancellationToken cancellationToken = default)
    {
        var token = Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(32));

        memoryCache.Set(GetKey(token), user, new MemoryCacheEntryOptions
        {
            SlidingExpiration = SessionDuration
        });

        return Task.FromResult(token);
    }

    public Task<SessionUser?> GetAsync(string token, CancellationToken cancellationToken = default) =>
        Task.FromResult(memoryCache.Get<SessionUser>(GetKey(token)));

    public Task RemoveAsync(string token, CancellationToken cancellationToken = default)
    {
        memoryCache.Remove(GetKey(token));

        return Task.CompletedTask;
    }

    private static string GetKey(string token) => $"oturum:{token}";
}
