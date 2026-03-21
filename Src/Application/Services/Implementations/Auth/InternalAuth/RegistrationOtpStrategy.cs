using Application.Services.Interfaces.Auth.InternalAuth;
using Application.DTOs.Auth.InternalAuth;

namespace Application.Services.Implementations.Auth.InternalAuth;

public class RegistrationOtpStrategy : IOtpStrategy<RegistrationOtpPayload>
{
    public string KeyPrefix => "new_user:";
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);

}