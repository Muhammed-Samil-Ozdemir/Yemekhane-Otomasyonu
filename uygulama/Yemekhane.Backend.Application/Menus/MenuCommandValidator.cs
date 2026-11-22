namespace Yemekhane.Backend.Application.Menus;

internal static class MenuCommandValidator
{
    public static string[] Validate(DateOnly date, decimal studentPrice, decimal staffPrice, IReadOnlyCollection<int> foodIds)
    {
        var errors = new List<string>();

        if (date < DateOnly.FromDateTime(DateTime.Now))
            errors.Add("Geçmiş bir güne menü girilemez.");

        if (studentPrice <= 0 || staffPrice <= 0)
            errors.Add("Öğrenci ve personel fiyatı sıfırdan büyük olmalıdır.");

        if (foodIds.Count == 0)
            errors.Add("Menüye en az bir yemek ekleyin.");

        return [.. errors];
    }
}
