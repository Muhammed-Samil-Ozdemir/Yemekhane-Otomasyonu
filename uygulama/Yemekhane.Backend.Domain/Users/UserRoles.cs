namespace Yemekhane.Backend.Domain.Users;

public static class UserRoles
{
    public const string Student = "ogrenci";
    public const string Staff = "personel";
    public const string Admin = "yonetici";

    public static string GetText(string role) => role switch
    {
        Student => "Öğrenci",
        Staff => "Personel",
        Admin => "Yönetici",
        _ => role
    };
}
