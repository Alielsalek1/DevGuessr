using Application.Common.Options;
using Infrastructure.Common.Options;
using API.Utils;

namespace API.Extensions;

public static class OptionsExtensions
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services)
    {
        // Use the original author's loader to populate the Options classes
        // This keeps the validation logic centralized in EnvironmentVariableLoader
        
        services.Configure<JwtOptions>(options =>
        {
            options.Key = EnvironmentVariableLoader.GetRequired("JWT_KEY");
            options.Issuer = EnvironmentVariableLoader.GetRequired("JWT_ISSUER");
            options.Audience = EnvironmentVariableLoader.GetRequired("JWT_AUDIENCE");
            
            var duration = Environment.GetEnvironmentVariable("JWT_DURATION_IN_MINUTES");
            options.DurationInMinutes = int.TryParse(duration, out var d) ? d : 60;
        });

        services.Configure<EmailOptions>(options =>
        {
            var emailConfig = EnvironmentVariableLoader.GetEmailConfig();
            options.Host = emailConfig["Host"];
            options.Port = int.Parse(emailConfig["Port"]);
            options.Username = emailConfig["Username"];
            options.Password = emailConfig["Password"];
            options.From = emailConfig["From"];
            options.EnableSsl = bool.Parse(emailConfig["EnableSsl"]);
        });

        services.Configure<DatabaseOptions>(options =>
        {
            options.ConnectionString = EnvironmentVariableLoader.GetRequired("CONNECTION_STRING");
        });

        services.Configure<RedisOptions>(options =>
        {
            options.ConnectionString = EnvironmentVariableLoader.GetRequired("REDIS_CONNECTION_STRING");
        });

        services.Configure<RabbitMqOptions>(options =>
        {
            options.Host = EnvironmentVariableLoader.GetRequired("RABBITMQ_HOST");
            options.Port = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672";
            options.Username = EnvironmentVariableLoader.GetRequired("RABBITMQ_USERNAME");
            options.Password = EnvironmentVariableLoader.GetRequired("RABBITMQ_PASSWORD");
        });

        return services;
    }
}
