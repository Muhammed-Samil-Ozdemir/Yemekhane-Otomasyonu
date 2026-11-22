namespace Yemekhane.Backend.Application.Common;

public static class DateOnlyExtensions
{
    public static DateTime ToDateTime(this DateOnly date) =>
        date.ToDateTime(TimeOnly.MinValue);
}
