namespace Infrastructure.Common.Options;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";
    public string Host { get; set; } = string.Empty;
    public string Port { get; set; } = "5672";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
