using AWS_ChatService_Infrastructure.Interfaces;
using Google.Apis.Auth;

namespace AWS_ChatService_Infrastructure.Services;

public class GoogleAuthService : IGoogleAuthService
{
    public Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken)
    {
        throw new NotImplementedException();
    }
}