using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace API.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            // 1. Add general API info
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Backend Template API",
                    Version = "v1",
                    Description = "The Architect's Forge - A high-performance, resilient backend template with Clean Architecture",
                    Contact = new OpenApiContact
                    {
                        Name = "Development Team",
                        Email = "dev@example.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT"
                    }
                };
                return Task.CompletedTask;
            });

            // 2. Add JWT Bearer Security Scheme definition
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token in the format: Bearer {token}"
                });
                return Task.CompletedTask;
            });

            // 3. Force clean Operation IDs (Controller_Action)
            // Using a more unique format to avoid collisions which might cause them to be nulled
            options.AddOperationTransformer((operation, context, cancellationToken) =>
            {
                if (context.Description.ActionDescriptor is ControllerActionDescriptor desc)
                {
                    operation.OperationId = $"{desc.ControllerName}_{desc.ActionName}";
                }
                else if (context.Description.ActionDescriptor.RouteValues.TryGetValue("action", out var actionName))
                {
                    operation.OperationId = actionName;
                }
                return Task.CompletedTask;
            });

            // 4. Apply the Security Requirement to operations that require authorization
            options.AddOperationTransformer((operation, context, cancellationToken) =>
            {
                var metadata = context.Description.ActionDescriptor.EndpointMetadata;
                
                // If the endpoint has [Authorize] (IAuthorizeData) AND NOT [AllowAnonymous] (IAllowAnonymous)
                if (metadata.OfType<IAuthorizeData>().Any() && !metadata.OfType<IAllowAnonymous>().Any())
                {
                    operation.Security = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement
                        {
                            [new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            }] = Array.Empty<string>()
                        }
                    };
                }
                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static IEndpointRouteBuilder MapOpenApiDocumentation(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapOpenApi();
        return endpoints;
    }

    public static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app)
    {
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Backend Template API v1.0");
            options.RoutePrefix = "api-docs";
            options.DefaultModelsExpandDepth(2);
            options.DefaultModelExpandDepth(1);
            options.DisplayOperationId();
        });

        return app;
    }
}
