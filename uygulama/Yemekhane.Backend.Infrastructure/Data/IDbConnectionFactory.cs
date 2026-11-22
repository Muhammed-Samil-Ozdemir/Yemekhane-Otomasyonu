using MySqlConnector;

namespace Yemekhane.Backend.Infrastructure.Data;

public interface IDbConnectionFactory
{
    Task<MySqlConnection> OpenPrimaryAsync(CancellationToken cancellationToken);

    Task<MySqlConnection> OpenReplicaAsync(CancellationToken cancellationToken);
}
