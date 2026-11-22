using Microsoft.Extensions.Configuration;

namespace Yemekhane.Backend.Infrastructure.Cache;

public sealed class RedisOption
{
    [ConfigurationKeyName("REDIS_HOST")]
    public string Host { get; set; } = "redis";

    [ConfigurationKeyName("REDIS_PORT")]
    public int Port { get; set; } = 6379;

    public string ConnectionString => $"{Host}:{Port},abortConnect=false";
}
