using Microsoft.Extensions.Logging;
using Yemekhane.Backend.Application.Cache;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Domain.Menus;

namespace Yemekhane.Backend.Application.Menus;

internal sealed class MenuReader(
    IMenuRepository menuRepository,
    ICacheService cacheService,
    ILogger<MenuReader> logger)
{
    public async Task<Menu?> GetAsync(int cafeteriaId, DateOnly date, int mealId, CancellationToken cancellationToken)
    {
        var key = CacheKeys.Menu(cafeteriaId, date, mealId);

        var cachedMenu = await cacheService.GetAsync<Menu>(key, cancellationToken);
        if (cachedMenu is not null)
        {
            logger.LogInformation("Menü Redis önbelleğinden okundu: {Key}", key);
            return cachedMenu;
        }

        var menu = await menuRepository.GetAsync(cafeteriaId, date.ToDateTime(), mealId, cancellationToken);
        if (menu is null)
            return null;

        await cacheService.SetAsync(key, menu, GetExpiration(date, DateTime.Now), cancellationToken);
        logger.LogInformation("Menü veritabanından (replika) okundu ve önbelleğe yazıldı: {Key}", key);

        return menu;
    }

    private static TimeSpan GetExpiration(DateOnly menuDate, DateTime now)
    {
        var today = DateOnly.FromDateTime(now);
        var lastDay = menuDate > today ? menuDate : today;

        return lastDay.AddDays(1).ToDateTime() - now;
    }
}
