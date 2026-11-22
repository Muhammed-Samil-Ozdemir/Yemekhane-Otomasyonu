using Dapper;
using Yemekhane.Backend.Domain.Cafeterias;
using Yemekhane.Backend.Infrastructure.Data;

namespace Yemekhane.Backend.Infrastructure.Cafeterias;

public sealed class CafeteriaRepository(
    IDbConnectionFactory connectionFactory) : ICafeteriaRepository
{
    public async Task<List<Cafeteria>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id       AS Id,
                   ad       AS Name,
                   konum    AS Location,
                   kapasite AS Capacity
            FROM yemekhaneler
            ORDER BY id
            """;

        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        var cafeterias = await connection.QueryAsync<Cafeteria>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return cafeterias.ToList();
    }
}
