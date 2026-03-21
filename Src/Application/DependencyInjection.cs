using Application.Services.Interfaces;
using Application.Services.Implementations;
using Application.Validators.Auth;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Application.Services.Implementations.Auth;
using Application.Services.Implementations.Auth.InternalAuth;
using Application.Services.Interfaces.Auth;
using Application.Services.Interfaces.Auth.InternalAuth;
using Application.Services.Implementations.Misc;
using Application.DTOs.Auth.InternalAuth;
using FluentEmail.Smtp;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core;
using Microsoft.Extensions.Options;
using Application.Common.Options;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidation();
        services.AddEmailServices();
        services.AddApplicationServices();

        return services;
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterRequestDtoValidator>();
        return services;
    }

    private static IServiceCollection AddEmailServices(this IServiceCollection services)
    {
        services.AddScoped<ISender>(sp =>
        {
            var emailOptions = sp.GetRequiredService<IOptions<EmailOptions>>().Value;
            return new SmtpSender(new System.Net.Mail.SmtpClient(emailOptions.Host)
            {
                Port = emailOptions.Port,
                Credentials = new System.Net.NetworkCredential(emailOptions.Username, emailOptions.Password),
                EnableSsl = emailOptions.EnableSsl
            });
        });

        // We also need to configure FluentEmail itself with the 'From' address
        services.AddFluentEmail(string.Empty) // Placeholder
            .AddSmtpSender(string.Empty, 25); // Placeholder

        // Better approach for FluentEmail with Options:
        services.AddScoped<IFluentEmail>(sp =>
        {
            var emailOptions = sp.GetRequiredService<IOptions<EmailOptions>>().Value;
            var sender = sp.GetRequiredService<ISender>();
            return new Email(emailOptions.From)
            {
                Sender = sender
            };
        });

        services.AddScoped<RegisterationConfirmationEmailSender>();
        services.AddScoped<NewDeviceConfirmationEmailSender>();
        services.AddScoped<PasswordResetEmailSender>();
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
}
