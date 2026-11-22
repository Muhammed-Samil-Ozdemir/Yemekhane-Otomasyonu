namespace Yemekhane.Backend.Domain.Balances;

public static class BalanceMovementTypes
{
    public const string Load = "yukleme";
    public const string Reservation = "rezervasyon";
    public const string Refund = "iade";

    public static string GetText(string type) => type switch
    {
        Load => "Yükleme",
        Reservation => "Rezervasyon",
        Refund => "İade",
        _ => type
    };
}
