using Application.Services.Interfaces.Auth.InternalAuth;
using Application.DTOs.Auth.InternalAuth;

namespace Application.Services.Implementations.Auth.InternalAuth;

public class PasswordResetOtpStrategy : IOtpStrategy<PasswordResetOtpPayload>
{
    public string KeyPrefix => "reset_password:";
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);

}