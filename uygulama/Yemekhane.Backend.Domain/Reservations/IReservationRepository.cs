namespace Yemekhane.Backend.Domain.Reservations;

public interface IReservationRepository
{
    Task<ReservationReceipt> CreateAsync(int userId, int menuId, string portion, CancellationToken cancellationToken);

    Task<ReservationReceipt> CancelAsync(int userId, int reservationId, CancellationToken cancellationToken);

    Task<TurnstileReceipt> PassTurnstileAsync(int userId, int cafeteriaId, DateTime? time, CancellationToken cancellationToken);

    Task<List<Reservation>> GetByUserAsync(int userId, CancellationToken cancellationToken);

    Task<List<Reservation>> GetByUserBetweenAsync(int userId, int cafeteriaId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}
