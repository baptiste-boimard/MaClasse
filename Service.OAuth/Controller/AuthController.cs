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
    private readonly ISessionRepository _sessionRepository;

    public AuthController(
        ServiceHashUrl serviceHashUrl,
        IConfiguration configuration,
        ValidateGoogleTokenService validateGoogleTokenService,
        IAuthRepository authRepository,
        ISessionRepository sessionRepository)
    {
        _serviceHashUrl = serviceHashUrl;
        _configuration = configuration;
        _validateGoogleTokenService = validateGoogleTokenService;
        _authRepository = authRepository;
        _sessionRepository = sessionRepository;
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
            //* Je regarde si une ancienne session existe pour ce user
            var alreadySession = await _sessionRepository.GetSessionByUserId(user.Id);

            if (alreadySession != null)
            {
                //* Je suprrime l'ancienne session
                var deleteSession = await _sessionRepository.DeleteSessionData(alreadySession);

                if (deleteSession == null) return Unauthorized();
            }
            
            //* Création du token de session
            var sessionTokenLogin = Guid.NewGuid().ToString();

            var newSessionTokenLogin = new SessionData
            {
                Token = sessionTokenLogin,
                UserId = existingUser.Id,
                Role = existingUser.Role,
                Expiration = DateTime.UtcNow.AddHours(3)
            };
            
            //* On le stock dans la table Session
            var sessionSaveLogin = await _sessionRepository.SaveNewSession(newSessionTokenLogin);
            
            if (sessionSaveLogin != null)
            {
                //* Envoi du cookie de session
                Response.Cookies.Append("MaClasseAuth", sessionTokenLogin, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(3)
                });
                
                _returnResponse = new AuthReturn
                {
                    IsNewUser = false,
                    User = existingUser,
                    IdSession = sessionSaveLogin.Token

                }; 
            
                return Ok(_returnResponse);
            }

            return Unauthorized();
        }
        
        //* Si l'utilisateur n'existe pas il faut le créer en BDD
        var newUser = await _authRepository.AddUser(user);
        
        //* Création du token de session
        var sessionTokenSignup = Guid.NewGuid().ToString();

        var newSessionTokenSignup = new SessionData
        {
            Token = sessionTokenSignup,
            UserId = newUser.Id,
            Role = "",
            Expiration = DateTime.UtcNow.AddHours(3)
        };
        
        //* On le stock dans la table Session
        var sessionSaveSignup = await _sessionRepository.SaveNewSession(newSessionTokenSignup);

        if (sessionSaveSignup != null)
        {
            //* On le place dans le cookies renvoyé
            Response.Cookies.Append("MaClasseAuth", sessionSaveSignup.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(3)
            });
        
            _returnResponse = new AuthReturn
            {
                IsNewUser = true,
                User = newUser,
                IdSession = sessionSaveSignup.Token
            };
            
            return Ok(_returnResponse);
        }

        return Unauthorized();

    }

    [HttpPost]
    [Route("finished-signup")]
    public async Task<IActionResult> FinishedSignUp([FromBody] CompleteProfileRequest request)
    {
        //* Récupérer la valeur du cookie
        var cookieValue = Request.Cookies["MaClasseAuth"];   

        //* Avec l'id de session on réucpére le userId
        var userSession = await _sessionRepository.GetUserIdByCookies(cookieValue);

        userSession.Role = request.Role;
        //* Je mets a jour mon cookies de Session avec le Role
        var updateSession = await _sessionRepository.UpdateSession(userSession);

        if (updateSession == null) return Unauthorized();
        
        //* Je récupére le user concerné
        var updateUser = await _authRepository.GetOneUserByGoogleId(updateSession.UserId);

        updateUser.Role = updateSession.Role;
        updateUser.UpdatedAt = DateTime.UtcNow;

        //* Si j'ai bien un user j'update son role
        var updatedUser = await _authRepository.UpdateUser(updateUser);

        if (updatedUser != null)
        {
            _returnResponse = new AuthReturn
            {
                IsNewUser = false,
                User = updateUser,
                IdSession = userSession.Token
            };
            
            return Ok(_returnResponse);
        }
        
        return Unauthorized();
    }
    
    [HttpPost]
    [Route("refresh-user")]
    public async Task<IActionResult> GetUser([FromBody] GoogleTokenRequest request)
    {
        //* Je récupére le userId avec le token de session
        var userSession = await _sessionRepository.GetUserIdByCookies(request.Token);

        if (userSession == null) return Unauthorized();
        
        //* Je vérifie la validité de la session
        if(userSession.Expiration < DateTime.UtcNow)
        {
            return Unauthorized();
        }

        //* Je récupére le user concerné
        var user = await _authRepository.GetOneUserByGoogleId(userSession.UserId);

        if (user == null) return Unauthorized();

        _returnResponse = new AuthReturn
        {
            IsNewUser = false,
            User = user,
            IdSession = userSession.Token
        };
        
        return Ok(_returnResponse);
    }
}





