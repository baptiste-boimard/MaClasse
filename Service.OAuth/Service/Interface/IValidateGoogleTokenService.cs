using Google.Apis.Auth;

namespace Service.OAuth.Service.Interface;

public interface IValidateGoogleTokenService
{
  Task<GoogleJsonWebSignature.Payload?> ValidateGoogleToken(string? token);
}