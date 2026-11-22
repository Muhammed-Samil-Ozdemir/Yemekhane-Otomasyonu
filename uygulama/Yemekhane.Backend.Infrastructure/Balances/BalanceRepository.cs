using Dapper;
using Yemekhane.Backend.Domain.Balances;
using Yemekhane.Backend.Infrastructure.Data;

namespace Yemekhane.Backend.Infrastructure.Balances;

public sealed class BalanceRepository(
    IDbConnectionFactory connectionFactory) : IBalanceRepository
{
    public async Task<BalanceLoadReceipt> LoadAsync(int userId, decimal amount, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenPrimaryAsync(cancellationToken);

        var (movementId, balance) = await connection.CallProcedureAsync<(int, decimal)>(
            "CALL sp_bakiye_yukle(@UserId, @Amount)",
            new { UserId = userId, Amount = amount },
            cancellationToken);

        return new BalanceLoadReceipt(movementId, balance);
    }

    public async Task<decimal> GetBalanceAsync(int userId, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<decimal>(new CommandDefinition(
            "SELECT bakiye FROM kullanicilar WHERE id = @UserId",
            new { UserId = userId },
            cancellationToken: cancellationToken));
    }

    public async Task<List<BalanceMovement>> GetMovementsAsync(int userId, int count, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id             AS Id,
                   tur            AS Type,
                   tutar          AS Amount,
                   rezervasyon_id AS ReservationId,
                   aciklama       AS Description,
                   olusturma      AS CreatedAt
            FROM bakiye_hareketleri
            WHERE kullanici_id = @UserId
            ORDER BY olusturma DESC, id DESC
            LIMIT @Count
            """;

        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        var movements = await connection.QueryAsync<BalanceMovement>(new CommandDefinition(
            sql,
            new { UserId = userId, Count = count },
            cancellationToken: cancellationToken));

        return movements.ToList();
    }
}
