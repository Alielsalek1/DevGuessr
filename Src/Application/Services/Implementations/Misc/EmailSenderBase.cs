using FluentEmail.Core;

namespace Application.Services.Implementations;

public abstract class EmailSenderBase(IFluentEmail fluentEmail)
{
    protected readonly IFluentEmail _fluentEmail = fluentEmail;

    // The "Template Method" - defines the flow.
    public async Task SendAsync(string recipient, string token, CancellationToken cancellationToken)
    {
        var subject = GetSubject();
        var body = GetBody(token);

        await ExecuteSendAsync(recipient, subject, body, cancellationToken);
    }

    protected abstract string GetSubject();
    protected abstract string GetBody(string token);

    // Common implementation logic
    protected virtual async Task ExecuteSendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        await _fluentEmail
            .To(to)
            .Subject(subject)
            .Body(body, isHtml: true)
            .SendAsync(cancellationToken);
    }
}
