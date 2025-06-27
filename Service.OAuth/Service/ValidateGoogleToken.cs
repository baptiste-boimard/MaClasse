using Google.Apis.Auth;
using Service.OAuth.Service.Interface;

namespace Service.OAuth.Service;

public class ValidateGoogleTokenService : IValidateGoogleTokenService
{
    private readonly IConfiguration _configuration;

    public ValidateGoogleTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<GoogleJsonWebSignature.Payload?> ValidateGoogleToken(string? token)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { $"{_configuration["Authentication:Google:ClientId"]}" }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
            return payload;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Erreur validation Google Token: {ex.Message}");
            return null;
        }
    }
}