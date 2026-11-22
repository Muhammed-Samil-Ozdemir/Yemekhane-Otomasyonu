using Microsoft.Extensions.Logging;
using Yemekhane.Backend.Application.Cache;
using Yemekhane.Backend.Domain.Menus;

namespace Yemekhane.Backend.Application.Menus;

internal sealed class MenuCacheInvalidator(
    ICacheService cacheService,
    ILogger<MenuCacheInvalidator> logger)
{
    public async Task InvalidateAsync(IEnumerable<MenuDefinition> menus, CancellationToken cancellationToken)
    {
        var keys = menus
            .Select(menu => CacheKeys.Menu(menu.CafeteriaId, DateOnly.FromDateTime(menu.Date), menu.MealId))
            .Distinct();

        foreach (var key in keys)
        {
            await cacheService.RemoveAsync(key, cancellationToken);
            logger.LogInformation("Menü değişti, önbellek kaydı silindi: {Key}", key);
        }
    }
}
