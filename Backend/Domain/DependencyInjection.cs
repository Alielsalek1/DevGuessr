using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Domain layer typically contains only models, enums, and exceptions
        // No service registrations needed at this time
        
        return services;
    }
}
