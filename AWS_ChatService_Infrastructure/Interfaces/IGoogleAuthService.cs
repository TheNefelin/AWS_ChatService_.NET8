using Google.Apis.Auth;

namespace AWS_ChatService_Infrastructure.Interfaces;

public interface IGoogleAuthService
{
    Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken);
}