using Google.Apis.Auth;
using MaClasse.Shared.Models;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Mvc;
using Service.OAuth.Interfaces;
using Service.OAuth.Repositories;
using Service.OAuth.Service;

namespace Service.OAuth.Controller;

[ApiController]
[Route("api")]
public class AuthController: ControllerBase
{
    private readonly ServiceHashUrl _serviceHashUrl;
    private readonly IConfiguration _configuration;
    private readonly ValidateGoogleTokenService _validateGoogleTokenService;
    private readonly IAuthRepository _authRepository;

    public AuthController(
        ServiceHashUrl serviceHashUrl,
        IConfiguration configuration,
        ValidateGoogleTokenService validateGoogleTokenService,
        IAuthRepository authRepository)
    {
        _serviceHashUrl = serviceHashUrl;
        _configuration = configuration;
        _validateGoogleTokenService = validateGoogleTokenService;
        _authRepository = authRepository;
    }

    private AuthReturn _returnResponse = new();
    
    [HttpPost]
    [Route("google-login")]
    public async Task<IActionResult> GoogleLogin(GoogleTokenRequest request)
    { 
        var payload = await _validateGoogleTokenService.ValidateGoogleToken(request.Token);
        if (payload == null)
        {
            return Unauthorized("Token invalide.");
        }

        var user = new UserProfile
        {
            Id = payload.Subject,
            Email = payload.Email,
            Name = payload.Name,
            GivenName = payload.GivenName,
            FamilyName = payload.FamilyName,
            Picture = payload.Picture
        };
        
        //* On recherche dans la base de données si l'utilisateur existe deja
        var existingUser = await _authRepository.GetOneUserByGoogleId(user.Id);

        if (existingUser != null)
        {
            _returnResponse = new AuthReturn
            {
                IsNewUser = true,
                User = user
            }; 
            
            return Ok(_returnResponse);
        }
        
        //* Si l'utilisateur n'existe pas il faut le créer en BDD
        var newUser = await _authRepository.AddUser(user);

        _returnResponse = new AuthReturn
        {
            IsNewUser = true,
            User = newUser
        };
        
        //! Gestion de l'erreur d'enregistrement ????
        
        return Ok(_returnResponse);
    }

    [HttpPost]
    [Route("finished-signup")]
    public async Task<IActionResult> FinishedSignUp([FromBody] CompleteProfileRequest request)
    {
        //! Implementer la mise a  jour du profil dans la base de données + création idendifiant
        return Ok(request);
    }
}





