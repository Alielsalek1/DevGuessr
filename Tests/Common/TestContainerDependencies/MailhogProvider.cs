using Tests.MailHog;
using DotNet.Testcontainers.Containers;

namespace Tests.Common.TestContainerDependencies;

public class MailhogProvider
{
    private readonly IContainer _container;

    public MailhogProvider(IContainer container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    public MailhogClient CreateClient()
    {
        var httpPort = _container.GetMappedPublicPort(8025);
        return new MailhogClient($"http://localhost:{httpPort}");
    }

    public async Task<MailhogMessagesResponse> GetAllMessagesAsync()
    {
        return await CreateClient().GetMessagesAsync();
    }

    public int GetSmtpPort() => _container.GetMappedPublicPort(1025);
}
