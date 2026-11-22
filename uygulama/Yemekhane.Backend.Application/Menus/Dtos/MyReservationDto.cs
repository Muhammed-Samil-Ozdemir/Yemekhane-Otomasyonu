namespace Yemekhane.Backend.Application.Menus.Dtos;

public sealed record MyReservationDto
{
    public int Id { get; init; }
    public string Status { get; init; } = default!;
    public string StatusText { get; init; } = default!;
    public string Portion { get; init; } = default!;
    public string PortionText { get; init; } = default!;
}
