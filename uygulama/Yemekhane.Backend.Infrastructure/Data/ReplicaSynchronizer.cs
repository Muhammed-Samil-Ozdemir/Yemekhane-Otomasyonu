using Dapper;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace Yemekhane.Backend.Infrastructure.Data;

public sealed class ReplicaSynchronizer(
    IDbConnectionFactory connectionFactory,
    ILogger<ReplicaSynchronizer> logger)
{
    private const int TimeoutSeconds = 5;

    public async Task WaitAsync(MySqlConnection primaryConnection, CancellationToken cancellationToken)
    {
        var gtidSet = await primaryConnection.ExecuteScalarAsync<string>(new CommandDefinition(
            "SELECT @@GLOBAL.gtid_executed",
            cancellationToken: cancellationToken));

        await using var replicaConnection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        var timedOut = await replicaConnection.ExecuteScalarAsync<int>(new CommandDefinition(
            "SELECT WAIT_FOR_EXECUTED_GTID_SET(@GtidSet, @Timeout)",
            new { GtidSet = gtidSet, Timeout = TimeoutSeconds },
            cancellationToken: cancellationToken));

        if (timedOut == 0)
            logger.LogInformation("Replika değişikliği uyguladı, önbellek güvenle silinebilir.");
        else
            logger.LogWarning("Replika {Timeout} saniye içinde yetişmedi.", TimeoutSeconds);
    }
}
