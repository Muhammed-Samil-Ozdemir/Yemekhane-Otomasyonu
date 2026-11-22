using Dapper;
using Yemekhane.Backend.Domain.Reservations;
using Yemekhane.Backend.Infrastructure.Data;

namespace Yemekhane.Backend.Infrastructure.Reservations;

public sealed class ReservationRepository(
    IDbConnectionFactory connectionFactory) : IReservationRepository
{
    private const string SelectSql = """
        SELECT r.id                           AS Id,
               r.menu_id                      AS MenuId,
               m.tarih                        AS Date,
               m.ogun_id                      AS MealId,
               o.ad                           AS MealName,
               yh.ad                          AS CafeteriaName,
               TIMESTAMP(m.tarih, o.son_saat) AS ReservationDeadline,
               r.durum                        AS Status,
               r.porsiyon                     AS Portion,
               r.ucret                        AS Price,
               r.olusturma                    AS CreatedAt
        FROM rezervasyonlar r
                 JOIN menuler m ON m.id = r.menu_id
                 JOIN ogunler o ON o.id = m.ogun_id
                 JOIN yemekhaneler yh ON yh.id = m.yemekhane_id
        """;

    public async Task<ReservationReceipt> CreateAsync(int userId, int menuId, string portion, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenPrimaryAsync(cancellationToken);

        var (reservationId, price, balance) = await connection.CallProcedureAsync<(int, decimal, decimal)>(
            "CALL sp_rezervasyon_yap(@UserId, @MenuId, @Portion)",
            new { UserId = userId, MenuId = menuId, Portion = portion },
            cancellationToken);

        return new ReservationReceipt(reservationId, price, balance);
    }

    public async Task<ReservationReceipt> CancelAsync(int userId, int reservationId, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenPrimaryAsync(cancellationToken);

        var (cancelledId, refund, balance) = await connection.CallProcedureAsync<(int, decimal, decimal)>(
            "CALL sp_rezervasyon_iptal(@UserId, @ReservationId)",
            new { UserId = userId, ReservationId = reservationId },
            cancellationToken);

        return new ReservationReceipt(cancelledId, refund, balance);
    }

    public async Task<TurnstileReceipt> PassTurnstileAsync(int userId, int cafeteriaId, DateTime? time, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenPrimaryAsync(cancellationToken);

        var (reservationId, menuId, mealName) = await connection.CallProcedureAsync<(int, int, string)>(
            "CALL sp_turnike_gecis(@UserId, @CafeteriaId, @Time)",
            new { UserId = userId, CafeteriaId = cafeteriaId, Time = time },
            cancellationToken);

        return new TurnstileReceipt(reservationId, menuId, mealName);
    }

    public async Task<List<Reservation>> GetByUserAsync(int userId, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        var reservations = await connection.QueryAsync<Reservation>(new CommandDefinition(
            $"""
             {SelectSql}
             WHERE r.kullanici_id = @UserId
             ORDER BY m.tarih DESC, o.sira DESC
             """,
            new { UserId = userId },
            cancellationToken: cancellationToken));

        return reservations.ToList();
    }

    public async Task<List<Reservation>> GetByUserBetweenAsync(int userId, int cafeteriaId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        var reservations = await connection.QueryAsync<Reservation>(new CommandDefinition(
            $"""
             {SelectSql}
             WHERE r.kullanici_id = @UserId
               AND m.yemekhane_id = @CafeteriaId
               AND m.tarih BETWEEN @StartDate AND @EndDate
             """,
            new { UserId = userId, CafeteriaId = cafeteriaId, StartDate = startDate, EndDate = endDate },
            cancellationToken: cancellationToken));

        return reservations.ToList();
    }
}
