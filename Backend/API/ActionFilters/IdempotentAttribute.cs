using Microsoft.AspNetCore.Mvc.Filters;

namespace API.ActionFilters;
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IdempotentAttribute : Attribute, IFilterFactory
{
    public bool IsReusable => false;

    // Factory method to get the filter from DI
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IdempotencyFilter>();
    }
}