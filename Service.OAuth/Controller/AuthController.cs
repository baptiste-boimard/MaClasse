using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Lesson;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Mvc;
using Service.OAuth.Interfaces;
using Service.OAuth.Service;

namespace Service.OAuth.Controller;

[ApiController]
[Route("api")]
public class AuthController: ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ValidateGoogleTokenService _validateGoogleTokenService;
    private readonly IAuthRepository _authRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly UserServiceRattachment _userServiceRattachment;
    private readonly GenerateIdRole _generateIdRole;
    private readonly CreateDataService _createDataService;
    private readonly DeleteUserService _deleteUserService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IConfiguration configuration,
        ValidateGoogleTokenService validateGoogleTokenService,
        IAuthRepository authRepository,
        ISessionRepository sessionRepository,
        UserServiceRattachment userServiceRattachment,
        GenerateIdRole generateIdRole,
        CreateDataService createDataService,
        DeleteUserService deleteUserService,
        ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _validateGoogleTokenService = validateGoogleTokenService;
        _authRepository = authRepository;
        _sessionRepository = sessionRepository;
        _userServiceRattachment = userServiceRattachment;
        _generateIdRole = generateIdRole;
        _createDataService = createDataService;
        _deleteUserService = deleteUserService;
        _logger = logger;
    }

    private AuthReturn _returnResponse = new();
    private Scheduler newScheduler = new();
    private LessonBook newLessonBook = new();
    
    [HttpPost]
    [Route("google-login")]
    public async Task<IActionResult> GoogleLogin(GoogleTokenRequest request)
    {
        Console.WriteLine("coucoffu");
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
            
            //* Je récupére les données adjacentes
            
            newScheduler = await _createDataService.GetDataScheduler(existingUser.Id);
            
            if (sessionSaveLogin != null)
            {
                //* On rechercher les rattachements de l'utilisateur
                _returnResponse = await _userServiceRattachment.GetUserWithRattachment(
                    existingUser, false, sessionSaveLogin.Token, request.Token, newScheduler );
               
                return Ok(_returnResponse);
            }

            return Unauthorized();
        }
        
        //* Si l'utilisateur n'existe pas il faut le créer en BDD
        
        //* Création de l'idRole
        var idRole = await _generateIdRole.GenerateIdAsync();
        
        user.IdRole = idRole;
        
        var newUser = await _authRepository.AddUser(user);
        
        //* On crée également les éléments adjacent au user
        //* avec le CreateDataService
        //* le scheduler
        newScheduler = await _createDataService.CreateDataScheduler(newUser.Id);
        newLessonBook = await _createDataService.CreateDateLessonBook(newUser.Id);
        
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
                Expires = DateTimeOffset.UtcNow.AddMinutes(5)
            });

            _returnResponse = await _userServiceRattachment.GetUserWithRattachment(
                newUser, true, sessionSaveSignup.Token, request.Token, newScheduler);
            
            return Ok(_returnResponse);
        }

        return Unauthorized();

    }

    [HttpPost]
    [Route("finished-signup")]
    public async Task<IActionResult> FinishedSignUp([FromBody] SignupDialogResult result)
    {
        //* Récupérer la valeur du cookie
        // var cookieValue = Request.Cookies["MaClasseAuth"];   

        //* Avec l'id de session on réucpére le userId
        var userSession = await _sessionRepository.GetUserIdByCookies(result.IdSession);

        userSession.Role = result.Role;
        //* Je mets a jour mon cookies de Session avec le Role
        var updateSession = await _sessionRepository.UpdateSession(userSession);

        if (updateSession == null) return Unauthorized();
        
        //* Je récupére le user concerné
        var updateUser = await _authRepository.GetOneUserByGoogleId(updateSession.UserId);

        updateUser.Role = updateSession.Role;
        updateUser.Zone = result.Zone;
        updateUser.UpdatedAt = DateTime.UtcNow;

        //* Si j'ai bien un user j'update son role
        var updatedUser = await _authRepository.UpdateUser(updateUser);
        
        
        //* J'ajoute les vacances scolaire en fonction de la zone et je récupére le scheduler
        newScheduler = await _createDataService.AddHolidayToScheduler(updatedUser);
        
        
        //* Je récupére les données adjacentes
        // newScheduler = await _createDataService.GetDataScheduler(updatedUser.Id);

        if (updatedUser != null && newScheduler != null)
        {
            _returnResponse = await _userServiceRattachment.GetUserWithRattachment(
                updatedUser, false, userSession.Token, result.AccessToken, newScheduler);
            
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

        newScheduler = await _createDataService.GetDataScheduler(user.Id);
        
        if (user == null) return Unauthorized();

        _returnResponse = await _userServiceRattachment.GetUserWithRattachment(
            user, false, userSession.Token, request.Token, newScheduler);
        
        return Ok(_returnResponse);
    }

    [HttpPost]
    [Route("change-profil")]
    public async Task<IActionResult> ChangeRole([FromBody] ChangeProfilRequest request)
    {
        var existingSession = await _sessionRepository.GetUserIdByCookies(request.IdSession);

        if (existingSession != null)
        {
            var user = await _authRepository.GetOneUserByGoogleId(existingSession.UserId);

            if (user != null)
            {
                user.Zone = request.Zone;
                user.Role = request.Role;
                user.UpdatedAt = DateTime.UtcNow;
                var updatedUser = await _authRepository.UpdateUser(user);
                
                if (updatedUser != null)
                {
                    return Ok(updatedUser);
                }
                
                return Unauthorized();
            }
            
            return Unauthorized();
        }
        else
        {
            return Unauthorized();   
        }
    }
    
    [HttpPost]
    [Route("delete-user")]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
    {
        var existingSession = await _sessionRepository.GetUserIdByCookies(request.IdSession);
        
        if (existingSession != null)
        {
            var user = await _authRepository.GetOneUserByGoogleId(existingSession.UserId);
            
            //* je supprime la session
            var deleteSession = await _sessionRepository.DeleteSessionData(existingSession);
            
            if (deleteSession == null) return Unauthorized();
            
            if (user != null)
            {
                //* Je delete le user et ces data
                var deletedUser = await _authRepository.DeleteUser(user);
                
                //! Je delete agalement le scheduler et le lessonBook du user
                await _deleteUserService.DeleteLessonBook(user.Id);
                await _deleteUserService.DeleteScheduler(user.Id);
                
                if (deletedUser != null)
                {
                    return Ok(deletedUser);
                }
                
                return NotFound();
            }
            
            return Unauthorized();
        }
        else
        {
            return Unauthorized();   
        }
    }
}





