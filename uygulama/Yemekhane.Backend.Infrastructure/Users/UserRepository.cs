using Dapper;
using Yemekhane.Backend.Domain.Users;
using Yemekhane.Backend.Infrastructure.Data;

namespace Yemekhane.Backend.Infrastructure.Users;

public sealed class UserRepository(
    IDbConnectionFactory connectionFactory) : IUserRepository
{
    private const string SelectSql = """
        SELECT id         AS Id,
               ad         AS FirstName,
               soyad      AS LastName,
               eposta     AS Email,
               sifre_hash AS PasswordHash,
               rol        AS Role,
               ogrenci_no AS StudentNumber,
               bakiye     AS Balance,
               aktif      AS IsActive
        FROM kullanicilar
        """;

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<User>(new CommandDefinition(
            $"{SelectSql} WHERE eposta = @Email",
            new { Email = email },
            cancellationToken: cancellationToken));
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<User>(new CommandDefinition(
            $"{SelectSql} WHERE id = @Id",
            new { Id = id },
            cancellationToken: cancellationToken));
    }
}
