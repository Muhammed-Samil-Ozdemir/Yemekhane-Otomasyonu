using Dapper;
using MySqlConnector;
using Yemekhane.Backend.Domain.Abstractions;
using Yemekhane.Backend.Domain.Allergens;
using Yemekhane.Backend.Infrastructure.Data;

namespace Yemekhane.Backend.Infrastructure.Allergens;

public sealed class AllergenRepository(
    IDbConnectionFactory connectionFactory) : IAllergenRepository
{
    public async Task<List<Allergen>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id AS Id,
                   ad AS Name
            FROM alerjenler
            ORDER BY ad
            """;

        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        var allergens = await connection.QueryAsync<Allergen>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return allergens.ToList();
    }

    public async Task<List<Allergen>> GetUserAllergensAsync(int userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT a.id AS Id,
                   a.ad AS Name
            FROM kullanici_alerjenleri ka
                     JOIN alerjenler a ON a.id = ka.alerjen_id
            WHERE ka.kullanici_id = @UserId
            ORDER BY a.ad
            """;

        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        var allergens = await connection.QueryAsync<Allergen>(new CommandDefinition(
            sql,
            new { UserId = userId },
            cancellationToken: cancellationToken));

        return allergens.ToList();
    }

    public async Task SetUserAllergensAsync(int userId, IReadOnlyCollection<int> allergenIds, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenPrimaryAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await connection.ExecuteAsync(new CommandDefinition(
                "DELETE FROM kullanici_alerjenleri WHERE kullanici_id = @UserId",
                new { UserId = userId },
                transaction,
                cancellationToken: cancellationToken));

            await connection.ExecuteAsync(new CommandDefinition(
                "INSERT INTO kullanici_alerjenleri (kullanici_id, alerjen_id) VALUES (@UserId, @AllergenId)",
                allergenIds.Select(allergenId => new { UserId = userId, AllergenId = allergenId }),
                transaction,
                cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);
        }
        catch (MySqlException exception) when (exception.ErrorCode == MySqlErrorCode.NoReferencedRow2)
        {
            throw new BusinessRuleException("Seçilen alerjenlerden biri bulunamadı.");
        }
    }
}
