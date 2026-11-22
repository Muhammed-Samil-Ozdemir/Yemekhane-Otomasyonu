namespace Yemekhane.Backend.Application.Reservations.Dtos;

public sealed record ReservationReceiptDto(
    int ReservationId,
    decimal Amount,
    decimal Balance,
    string Message);
