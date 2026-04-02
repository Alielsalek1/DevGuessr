using Techdle.API.Middlewares;
using Npgsql;
using DotNetEnv;
using Serilog;
using Domain;
using Infrastructure;
using Application;
using API;
using API.Extensions;
using API.Utils;
using Hangfire;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Configuration Setup using the Original Author's Loader via Options Pattern
builder.Services.AddAppOptions();

// Configure Kestrel server limits
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
    serverOptions.Limits.MaxRequestHeadersTotalSize = 32 * 1024;
    serverOptions.Limits.MaxRequestHeaderCount = 100;
    serverOptions.Limits.MaxRequestLineSize = 8 * 1024;
});

// Configure Serilog
var loggerConfig = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day);

var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL");
if (builder.Environment.IsDevelopment() && !string.IsNullOrEmpty(seqUrl))
{
    loggerConfig = loggerConfig.WriteTo.Seq(seqUrl);
}
Log.Logger = loggerConfig.CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting web host");

    // Layer Registration
    builder.Services.AddDomain();
    builder.Services.AddInfrastructure();
    builder.Services.AddApplication();
    builder.Services.AddApiLayer(builder.Environment.IsDevelopment());

    // testing purposes only
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    // Health Checks - Using Loader to maintain consistent validation behavior
    var connectionString = EnvironmentVariableLoader.GetRequired("CONNECTION_STRING");
    var redisConnectionString = EnvironmentVariableLoader.GetRequired("REDIS_CONNECTION_STRING");

    builder.Services.AddHealthChecks()
        .AddNpgSql(sp => sp.GetRequiredService<NpgsqlDataSource>(), name: "database", tags: ["ready"])
        .AddRedis(redisConnectionString, name: "redis", tags: ["ready"])
        .AddHangfire(options =>
        {
            options.MinimumAvailableServers = 1;
        }, tags: ["ready"]);

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseForwardedHeaders();

    // testing purposes only
    app.UseCors("AllowAll");

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApiDocumentation();
        app.UseOpenApiDocumentation();
    }

    if (!app.Environment.IsEnvironment("Testing"))
    {
        app.UseRateLimiter();
    }

    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    app.UseHangfireDashboard("/hangfire");

    // Serve uploaded images (e.g. /uploads/...) as static files
    var uploadPath = EnvironmentVariableLoader.GetRequired("UPLOAD_PATH");
    var uploadBaseUrl = EnvironmentVariableLoader.GetRequired("UPLOAD_BASE_URL");
    
    var fullUploadPath = Path.Combine(builder.Environment.ContentRootPath, uploadPath);
    if (!Directory.Exists(fullUploadPath))
    {
        Directory.CreateDirectory(fullUploadPath);
    }

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(fullUploadPath),
        RequestPath = uploadBaseUrl 
    });

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecks("/health/auth", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                Status = report.Status.ToString(),
                Message = "Authenticated"
            };
            await context.Response.WriteAsJsonAsync(response);
        }
    }).RequireAuthorization();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
