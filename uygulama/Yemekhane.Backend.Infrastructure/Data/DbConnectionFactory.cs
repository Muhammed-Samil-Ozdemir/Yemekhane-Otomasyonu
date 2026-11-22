using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Yemekhane.Backend.Infrastructure.Data;

public sealed class DbConnectionFactory(
    IOptions<DatabaseOption> options,
    ILogger<DbConnectionFactory> logger) : IDbConnectionFactory
{
    public Task<MySqlConnection> OpenPrimaryAsync(CancellationToken cancellationToken) =>
        OpenAsync(options.Value.WriteConnectionString, "kaynak (mysql-primary)", cancellationToken);

    public Task<MySqlConnection> OpenReplicaAsync(CancellationToken cancellationToken) =>
        OpenAsync(options.Value.ReadConnectionString, "replika (mysql-replica)", cancellationToken);

    private async Task<MySqlConnection> OpenAsync(string connectionString, string server, CancellationToken cancellationToken)
    {
        var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        logger.LogDebug("Veritabanı bağlantısı: {Server}", server);

        return connection;
    }
}
