using Yemekhane.Backend.Domain.Abstractions;

namespace Yemekhane.Backend.Domain.Meals;

public sealed class Meal : Entity
{
    public string Name { get; set; } = default!;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public TimeSpan ReservationDeadline { get; set; }
    public int SortOrder { get; set; }
}
