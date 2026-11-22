using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Yemekhane.Backend.Infrastructure.Data;

public sealed class DatabaseOption
{
    [ConfigurationKeyName("DB_YAZMA_HOST")]
    public string WriteHost { get; set; } = "mysql-primary";

    [ConfigurationKeyName("DB_OKUMA_HOST")]
    public string ReadHost { get; set; } = "mysql-replica";

    [ConfigurationKeyName("DB_PORT")]
    public uint Port { get; set; } = 3306;

    [ConfigurationKeyName("DB_YAZMA_PORT")]
    public uint? WritePort { get; set; }

    [ConfigurationKeyName("DB_OKUMA_PORT")]
    public uint? ReadPort { get; set; }

    [ConfigurationKeyName("DB_ADI")]
    public string Database { get; set; } = "yemekhane";

    [ConfigurationKeyName("DB_KULLANICI")]
    public string User { get; set; } = default!;

    [ConfigurationKeyName("DB_SIFRE")]
    public string Password { get; set; } = default!;

    public string WriteConnectionString => BuildConnectionString(WriteHost, WritePort ?? Port);

    public string ReadConnectionString => BuildConnectionString(ReadHost, ReadPort ?? Port);

    private string BuildConnectionString(string host, uint port) =>
        new MySqlConnectionStringBuilder
        {
            Server = host,
            Port = port,
            Database = Database,
            UserID = User,
            Password = Password,
            CharacterSet = "utf8mb4",
            AllowPublicKeyRetrieval = true
        }.ConnectionString;
}
