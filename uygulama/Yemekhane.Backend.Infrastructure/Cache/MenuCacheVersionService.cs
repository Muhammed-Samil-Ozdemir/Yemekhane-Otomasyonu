using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Yemekhane.Backend.Application.Cache;

namespace Yemekhane.Backend.Infrastructure.Cache;

public sealed class MenuCacheVersionService(
    ICacheService cacheService,
    ILogger<MenuCacheVersionService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var currentVersion = await cacheService.GetAsync<int?>(CacheKeys.MenuCacheVersionKey, cancellationToken);
        if (currentVersion == CacheKeys.MenuCacheVersion)
            return;

        var removedCount = await cacheService.RemoveByPatternAsync(CacheKeys.MenuPattern, cancellationToken);
        await cacheService.SetAsync(CacheKeys.MenuCacheVersionKey, CacheKeys.MenuCacheVersion, null, cancellationToken);

        logger.LogInformation(
            "Menü önbelleği {Version}. sürüme geçti, eski yapıdaki {Count} menü kaydı Redis'ten silindi.",
            CacheKeys.MenuCacheVersion,
            removedCount);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
