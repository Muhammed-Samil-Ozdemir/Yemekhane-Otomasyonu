namespace Yemekhane.Backend.Domain.Reservations;

public static class ReservationStatuses
{
    public const string Active = "aktif";
    public const string Used = "kullanildi";
    public const string Cancelled = "iptal";

    public static string GetText(string status) => status switch
    {
        Active => "Aktif",
        Used => "Kullanıldı",
        Cancelled => "İptal",
        _ => status
    };
}
