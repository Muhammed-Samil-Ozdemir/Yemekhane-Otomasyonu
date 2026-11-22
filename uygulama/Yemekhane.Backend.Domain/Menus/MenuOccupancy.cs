namespace Yemekhane.Backend.Domain.Menus;

public sealed class MenuOccupancy
{
    public int MenuId { get; set; }
    public int Capacity { get; set; }
    public int ReservationCount { get; set; }
    public int FreeSeats { get; set; }
}
