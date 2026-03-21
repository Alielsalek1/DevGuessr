using Testcontainers.RabbitMq;

namespace Tests.Common.TestContainerDependencies;

public class RabbitMqProvider
{
    private readonly RabbitMqContainer _container;

    public RabbitMqProvider(RabbitMqContainer container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    public string GetConnectionString()
    {
        return _container.GetConnectionString();
    }
}
