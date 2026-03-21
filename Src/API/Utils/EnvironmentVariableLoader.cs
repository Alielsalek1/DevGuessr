namespace API.Utils;

public static class EnvironmentVariableLoader
{
    public static string GetRequired(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException($"{key} environment variable is not set.");
        }
        return value;
    }

    public static Dictionary<string, string> GetRequiredGroup(params string[] keys)
    {
        var config = new Dictionary<string, string>();
        var missing = new List<string>();

        foreach (var key in keys)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
            {
                missing.Add(key);
            }
            else
            {
                config[key] = value;
            }
        }

        if (missing.Count != 0)
        {
            throw new InvalidOperationException(
                $"Missing required environment variables: {string.Join(", ", missing)}");
        }

        return config;
    }

    public static Dictionary<string, string> GetEmailConfig()
    {
        var envVars = GetRequiredGroup(
            "EMAIL_HOST",
            "EMAIL_PORT",
            "EMAIL_USERNAME",
            "EMAIL_PASSWORD",
            "EMAIL_FROM",
            "EMAIL_ENABLE_SSL"
        );

        return new Dictionary<string, string>
        {
            { "Host", envVars["EMAIL_HOST"] },
            { "Port", envVars["EMAIL_PORT"] },
            { "Username", envVars["EMAIL_USERNAME"] },
            { "Password", envVars["EMAIL_PASSWORD"] },
            { "From", envVars["EMAIL_FROM"] },
            { "EnableSsl", envVars["EMAIL_ENABLE_SSL"] }
        };
    }
}
