using Application.Services.Interfaces;
using Google.Apis.Auth;

namespace Application.Services.Implementations;

public class GoogleAuthValidator : IGoogleAuthValidator
{
    public async Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken, GoogleJsonWebSignature.ValidationSettings settings)
    {
        return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
    }
}
