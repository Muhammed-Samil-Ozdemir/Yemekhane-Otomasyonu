namespace Yemekhane.Backend.Domain.Reservations;

public sealed record ReservationReceipt(int ReservationId, decimal Amount, decimal Balance);
