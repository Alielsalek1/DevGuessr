using Google.Apis.Auth;

namespace Application.Services.Interfaces;

public interface IGoogleAuthValidator
{
    Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken, GoogleJsonWebSignature.ValidationSettings settings);
}
