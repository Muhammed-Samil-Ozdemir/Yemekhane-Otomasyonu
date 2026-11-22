namespace Yemekhane.Backend.Domain.Reservations;

public static class Portions
{
    public const string Full = "tam";
    public const string Half = "yarim";

    public static string GetText(string portion) => portion switch
    {
        Full => "Tam porsiyon",
        Half => "Yarım porsiyon",
        _ => portion
    };
}
