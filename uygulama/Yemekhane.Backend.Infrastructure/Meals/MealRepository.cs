using Dapper;
using Yemekhane.Backend.Domain.Meals;
using Yemekhane.Backend.Infrastructure.Data;

namespace Yemekhane.Backend.Infrastructure.Meals;

public sealed class MealRepository(
    IDbConnectionFactory connectionFactory) : IMealRepository
{
    public async Task<List<Meal>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id               AS Id,
                   ad               AS Name,
                   servis_baslangic AS StartTime,
                   servis_bitis     AS EndTime,
                   son_saat         AS ReservationDeadline,
                   sira             AS SortOrder
            FROM ogunler
            ORDER BY sira
            """;

        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        var meals = await connection.QueryAsync<Meal>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return meals.ToList();
    }
}
