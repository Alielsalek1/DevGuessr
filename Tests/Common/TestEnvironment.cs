using Tests.Common.TestContainerDependencies;

namespace Tests.Common;

/// <summary>
/// Handles global environment configuration for integration tests.
/// </summary>
public static class TestEnvironment
{
    /// <summary>
    /// Sets up environment variables so Testcontainers can connect to Docker/Podman.
    /// </summary>
    public static void Configure()
    {
        // Configure Testcontainers for Docker environment
        // On Linux, the default Docker socket is /var/run/docker.sock
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOCKER_HOST")))
        {
            Environment.SetEnvironmentVariable("DOCKER_HOST", "unix:///var/run/docker.sock");
        }
        // Disables Ryuk (resource reaper) to avoid timeout issues
        Environment.SetEnvironmentVariable("TESTCONTAINERS_RYUK_DISABLED", "true");
    }

    /// <summary>
    /// Sets environment variables so the app uses the test database.
    /// </summary>
    public static void SetDatabaseEnvironmentVariables(string connectionString)
    {
        Environment.SetEnvironmentVariable("CONNECTION_STRING", connectionString);
    }

    /// <summary>
    /// Sets JWT environment variables for testing.
    /// </summary>
    public static void SetJwtEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("JWT_KEY", "b56aDg1e6yEWr66EvMyf26JtOHr28i58rVrsjggrwaQ=");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "TestIssuer");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "TestAudience");
        Environment.SetEnvironmentVariable("JWT_DURATION_IN_MINUTES", "60");
    }

    /// <summary>
    /// Sets environment variables so the app uses Mailhog for email delivery during tests.
    /// </summary>
    public static void SetEmailEnvironmentVariables(int mailhogSmtpPort)
    {
        Environment.SetEnvironmentVariable("EMAIL_HOST", "localhost");
        Environment.SetEnvironmentVariable("EMAIL_PORT", mailhogSmtpPort.ToString());
        Environment.SetEnvironmentVariable("EMAIL_USERNAME", "test");
        Environment.SetEnvironmentVariable("EMAIL_PASSWORD", "test");
        Environment.SetEnvironmentVariable("EMAIL_FROM", "noreply@test.com");
        Environment.SetEnvironmentVariable("EMAIL_ENABLE_SSL", "false");
    }

    /// <summary>
    /// Sets environment variables so the app uses the test Redis.
    /// </summary>
    public static void SetRedisEnvironmentVariables(string connectionString)
    {
        Environment.SetEnvironmentVariable("REDIS_CONNECTION_STRING", connectionString);
        Environment.SetEnvironmentVariable("REDIS_URL", connectionString);
    }

    /// <summary>
    /// Sets environment variables so the app uses the test RabbitMQ.
    /// </summary>
    public static void SetRabbitMqEnvironmentVariables(string connectionString)
    {
        // MassTransit RabbitMQ connection string is in URI format: rabbitmq://user:pass@host:port
        var uri = new Uri(connectionString);
        Environment.SetEnvironmentVariable("RABBITMQ_HOST", uri.Host);
        Environment.SetEnvironmentVariable("RABBITMQ_PORT", uri.Port.ToString());

        var userInfo = uri.UserInfo.Split(':');
        Environment.SetEnvironmentVariable("RABBITMQ_USERNAME", userInfo[0]);
        Environment.SetEnvironmentVariable("RABBITMQ_PASSWORD", userInfo.Length > 1 ? userInfo[1] : string.Empty);
    }

    /// <summary>
    /// Sets the ASPNETCORE_ENVIRONMENT to Testing.
    /// </summary>
    public static void SetAspNetCoreEnvironment()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    }

    /// <summary>
    /// Sets the Seq URL for logging to the test default.
    /// </summary>
    public static void SetSeqUrl()
    {
        SeqProvider.SetSeqUrl("http://localhost:5341");
    }

    /// <summary>
    /// Sets environment variables for uploading files.
    /// </summary>
    public static void SetUploadEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("UPLOAD_PATH", "test-uploads");
        Environment.SetEnvironmentVariable("UPLOAD_BASE_URL", "/uploads");
    }
}
