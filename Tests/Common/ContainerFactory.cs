using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Testcontainers.RabbitMq;

namespace Tests.Common;

/// <summary>
/// Factory for creating configured test containers for integration testing.
/// </summary>
public static class ContainerFactory
{
    /// <summary>
    /// Creates a PostgreSQL container for use in tests.
    /// </summary>
    public static PostgreSqlContainer CreatePostgreSqlContainer()
    {
        return new PostgreSqlBuilder("postgres:15-alpine")
            .WithReuse(true)
            .Build();
    }

    /// <summary>
    /// Creates a Mailhog container for capturing and inspecting emails during tests.
    /// </summary>
    public static IContainer CreateMailhogContainer()
    {
        return new ContainerBuilder("mailhog/mailhog:latest")
            .WithPortBinding(1025, true)  // SMTP port
            .WithPortBinding(8025, true)  // HTTP API port
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => 
                r.ForPort(8025).ForPath("/api/v2/messages")))
            .WithReuse(true)
            .Build();
    }

    /// <summary>
    /// Creates a Redis container for use in tests.
    /// </summary>
    public static RedisContainer CreateRedisContainer()
    {
        return new RedisBuilder("redis:7-alpine")
            .WithReuse(true)
            .Build();
    }

    /// <summary>
    /// Creates a RabbitMQ container for use in tests.
    /// </summary>
    public static RabbitMqContainer CreateRabbitMqContainer()
    {
        return new RabbitMqBuilder("rabbitmq:3-management-alpine")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilInternalTcpPortIsAvailable(5672)
                .UntilInternalTcpPortIsAvailable(15672))
            .WithReuse(true)
            .Build();
    }
}
