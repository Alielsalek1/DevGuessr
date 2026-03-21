using Techdle.API.Middlewares;
using DotNetEnv;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Domain;
using Infrastructure;
using Application;
using API;
using API.Utils;
using Hangfire;

Env.Load("../../.env");  // Load .env from root

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel server limits to prevent DoS attacks
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
    serverOptions.Limits.MaxRequestHeadersTotalSize = 32 * 1024; // 32 KB
    serverOptions.Limits.MaxRequestHeaderCount = 100; // Max number of headers
    serverOptions.Limits.MaxRequestLineSize = 8 * 1024; // 8 KB
});

// Configure Serilog
var loggerConfig = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day);
if (builder.Environment.IsDevelopment())
{
    loggerConfig = loggerConfig.WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ?? 
        throw new InvalidOperationException("SEQ_URL environment variable is not set."));
}
Log.Logger = loggerConfig.CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting web host");

    // Environment Variables - Consolidated Loading
    var jwtKey = EnvironmentVariableLoader.GetRequired("JWT_KEY");
    var jwtIssuer = EnvironmentVariableLoader.GetRequired("JWT_ISSUER");
    var jwtAudience = EnvironmentVariableLoader.GetRequired("JWT_AUDIENCE");
    var connectionString = EnvironmentVariableLoader.GetRequired("CONNECTION_STRING");
    var emailConfig = EnvironmentVariableLoader.GetEmailConfig();
    var redisConnectionString = EnvironmentVariableLoader.GetRequired("REDIS_CONNECTION_STRING");
    
    var rabbitMqHost = EnvironmentVariableLoader.GetRequired("RABBITMQ_HOST");
    var rabbitMqPort = EnvironmentVariableLoader.GetRequired("RABBITMQ_PORT");
    var rabbitMqUsername = EnvironmentVariableLoader.GetRequired("RABBITMQ_USERNAME");
    var rabbitMqPassword = EnvironmentVariableLoader.GetRequired("RABBITMQ_PASSWORD");

    // Layer Registration
    builder.Services.AddDomain();
    builder.Services.AddInfrastructure(connectionString);
    builder.Services.AddApplication(emailConfig, redisConnectionString, rabbitMqHost, rabbitMqPort, rabbitMqUsername, rabbitMqPassword);
    builder.Services.AddApiLayer(jwtKey, jwtIssuer, jwtAudience, connectionString, builder.Environment.IsDevelopment());

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

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseForwardedHeaders();

    // testing purposes only
    app.UseCors("AllowAll");

    // Enable Swagger UI only in development mode
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend Template API v1.0");
            options.RoutePrefix = "api-docs"; // Access Swagger at /api-docs
            options.DefaultModelsExpandDepth(2);
            options.DefaultModelExpandDepth(1);
            options.DisplayOperationId();
        });
    }

    if (!app.Environment.IsEnvironment("Testing"))
    {
        app.UseRateLimiter();
    }

    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    app.UseHangfireDashboard("/hangfire");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
    app.MapGet("/health/auth", [Authorize] () => Results.Ok(new { status = "Authenticated" }));
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { } // required for tests