using Yemekhane.Backend.Domain.Abstractions;

namespace Yemekhane.Backend.Domain.Reservations;

public sealed class Reservation : Entity
{
    public int MenuId { get; set; }
    public DateTime Date { get; set; }
    public int MealId { get; set; }
    public string MealName { get; set; } = default!;
    public string CafeteriaName { get; set; } = default!;
    public DateTime ReservationDeadline { get; set; }
    public string Status { get; set; } = default!;
    public string Portion { get; set; } = Portions.Full;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool CanCancel(DateTime now) =>
        Status == ReservationStatuses.Active && now < ReservationDeadline;
}
