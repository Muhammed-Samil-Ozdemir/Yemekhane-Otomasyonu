using System.Globalization;

namespace Yemekhane.Backend.Application.Cache;

public static class CacheKeys
{
    public const string MenuPattern = "menu:*";
    public const string MenuCacheVersionKey = "surum:menu";
    public const int MenuCacheVersion = 2;

    public static string Menu(int cafeteriaId, DateOnly date, int mealId) =>
        $"menu:{cafeteriaId}:{date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}:{mealId}";

    public static string Session(string token) =>
        $"oturum:{token}";
}
