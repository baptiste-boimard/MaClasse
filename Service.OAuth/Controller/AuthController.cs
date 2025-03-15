using Google.Apis.Auth;
using MaClasse.Shared.Models;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Mvc;
using Service.OAuth.Service;

namespace Service.OAuth.Controller;

[ApiController]
[Route("api")]
public class AuthController: ControllerBase
{
    private readonly ServiceHashUrl _serviceHashUrl;
    private readonly IConfiguration _configuration;
    private readonly ValidateGoogleTokenService _validateGoogleTokenService;

    public AuthController(
        ServiceHashUrl serviceHashUrl,
        IConfiguration configuration,
        ValidateGoogleTokenService validateGoogleTokenService)
    {
        _serviceHashUrl = serviceHashUrl;
        _configuration = configuration;
        _validateGoogleTokenService = validateGoogleTokenService;
    }
    
    [HttpPost]
    [Route("google-login")]
    public async Task<IActionResult> GoogleLogin(GoogleTokenRequest request)
    { 
        var payload = await _validateGoogleTokenService.ValidateGoogleToken(request.Token);
        if (payload == null)
        {
            return Unauthorized("Token invalide.");
        }

        var newUser = new UserProfile
        {
            Id = payload.Subject,
            Email = payload.Email,
            Name = payload.Name,
            GivenName = payload.GivenName,
            FamilyName = payload.FamilyName,
            Picture = payload.Picture
        };
        
        return Ok(newUser);
    }
}





