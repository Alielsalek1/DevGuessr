using Application.Services;
using Application.Services.Interfaces;
using Application.Services.Implementations;
using Application.Validators.Auth;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Application.Services.Implementations.Auth;
using Application.Services.Implementations.Auth.InternalAuth;
using Application.Services.Interfaces.Auth;
using Application.Services.Interfaces.Auth.InternalAuth;
using Application.Services.Implementations.Misc;
using Application.DTOs.Auth.InternalAuth;
using FluentEmail.Smtp;
using FluentEmail.Core.Interfaces;
using System.Net;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        Dictionary<string, string> emailConfig,
        string redisConnectionString,
        string rabbitMqHost,
        string rabbitMqPort,
        string rabbitMqUsername,
        string rabbitMqPassword)
    {
        services.AddValidation();
        services.AddEmailServices(emailConfig);
        services.AddCaching(redisConnectionString);
        services.AddApplicationServices();
        services.AddMessageBroker(rabbitMqHost, rabbitMqPort, rabbitMqUsername, rabbitMqPassword);

        return services;
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterRequestDtoValidator>();
        return services;
    }

    private static IServiceCollection AddEmailServices(this IServiceCollection services, Dictionary<string, string> emailConfig)
    {
        var enableSsl = bool.Parse(emailConfig["EnableSsl"]);
        services
            .AddFluentEmail(emailConfig["From"]);
        
        services.AddScoped<ISender>(sp => new SmtpSender(new System.Net.Mail.SmtpClient(emailConfig["Host"])
        {
            Port = int.Parse(emailConfig["Port"]),
            Credentials = new System.Net.NetworkCredential(emailConfig["Username"], emailConfig["Password"]),
            EnableSsl = enableSsl
        }));
        
        services.AddScoped<RegisterationConfirmationEmailSender>();
        services.AddScoped<NewDeviceConfirmationEmailSender>();
        services.AddScoped<PasswordResetEmailSender>();
        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services, string redisConnectionString)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "Techdle_";
        });
        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        // Otp Service & Strategies
        services.AddScoped(typeof(IOtpService<>), typeof(OtpService<>));
        services.AddScoped<IOtpStrategy<RegistrationOtpPayload>, RegistrationOtpStrategy>();
        services.AddScoped<IOtpStrategy<NewDeviceOtpPayload>, NewDeviceOtpStrategy>();
        services.AddScoped<IOtpStrategy<PasswordResetOtpPayload>, PasswordResetOtpStrategy>();

        // Auth Services
        services.AddScoped<ILoginThrottlingService, LoginThrottlingService>();
        services.AddScoped<JwtTokenProvider>();
        services.AddScoped<IInternalPasswordResetService, InternalPasswordResetService>();
        services.AddScoped<IInternalRegisterationService, InternalRegisterationService>();
        services.AddScoped<IInternalSessionService, InternalSessionService>();
        services.AddScoped<IInternalAuthFacadeService, InternalAuthFacadeService>();
        services.AddScoped<IInternalUserVerificationService, InternalUserVerificationService>();
        services.AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();
        services.AddScoped<IGoogleAuthValidator, GoogleAuthValidator>();
        services.AddScoped<IExternalAuthService, ExternalAuthService>();

        // User Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserFacadeService, UserFacadeService>();

        return services;
    }

    private static IServiceCollection AddMessageBroker(
        this IServiceCollection services,
        string rabbitMqHost,
        string rabbitMqPort,
        string rabbitMqUsername,
        string rabbitMqPassword)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqHost, ushort.TryParse(rabbitMqPort, out var port) ? port : (ushort)5672, "/", h =>
                {
                    h.Username(rabbitMqUsername);
                    h.Password(rabbitMqPassword);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}
