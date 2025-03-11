using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MaClasse.Shared.Models;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.OAuth.Service;
using static Service.OAuth.Service.JwtService;

namespace Service.OAuth.Controller;

[ApiController]
[Route("api")]
public class AuthController: ControllerBase
{
    private readonly UserManager<UserProfile> _userManager;
    private readonly JwtService _jwtService;
    private readonly ServiceHashUrl _serviceHashUrl;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<UserProfile> userManager,
        JwtService jwtService,
        ServiceHashUrl serviceHashUrl,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _serviceHashUrl = serviceHashUrl;
        _configuration = configuration;
    }
    
    [HttpGet("signin-google")]
    public IActionResult LoginGoogle(string returnUrl = "")
    {
        if (string.IsNullOrEmpty(returnUrl)) returnUrl = $"{_configuration["Url:Client"]}/login";
        var properties = new AuthenticationProperties { RedirectUri = returnUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("login")]
    public async Task<IActionResult> Login()
    {
        // var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        // if (!result.Succeeded)
        // {
        //     return Unauthorized();
        // }
        
        string encodedMessage;
        
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded)
        {
            return Unauthorized();
        }
        
        //* Récupérer les informations de Google
        var sub = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
        var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
        var pictureUrl = result.Principal.FindFirst("urn:google:picture")?.Value;
        
        var existingUser = await _userManager.FindByEmailAsync(email);
        
        if (existingUser == null)
        {
            ErrorOAuth errorOAuth = new ErrorOAuth
            {
                Error = true,
                Message = "Cet utilisateur n'est pas encore inscrit"
            };  
            
            var cryptedError = _serviceHashUrl.EncryptErrorOAuth(errorOAuth);
            
            encodedMessage = System.Net.WebUtility.UrlEncode(cryptedError);
            
            return Redirect($"{_configuration["Url:Client"]}/?error={encodedMessage}");
        }
        
        var loginUser = new UserProfile
        {
            UserName = sub,
            Email = email,
            Name = name,
            Picture = pictureUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Optionnel : nettoyer le cookie externe pour s'assurer qu'il n'interfère plus
        // await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        
        //* Création du token et envoie vers le client
        var token = _jwtService.GenerateJwtToken(loginUser);
        
        // var cryptedtoken = _serviceHashUrl.EncryptErrorOAuth(token);
        
        // //* Validation du token pour créer le ClaimsPrincipal
        // var tokenHandler = new JwtSecurityTokenHandler();
        // var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        // var validationParameters = new TokenValidationParameters
        // {
        //     ValidateIssuer = true,
        //     ValidateAudience = true,
        //     ValidateLifetime = true,
        //     ValidateIssuerSigningKey = true,
        //     ValidIssuer = _configuration["Jwt:Issuer"],
        //     ValidAudience = _configuration["Jwt:Audience"],
        //     IssuerSigningKey = new SymmetricSecurityKey(key)
        // };
        
        // Créez une identité avec le token comme claim
        var claims = new List<Claim>
        {
            new Claim("jwtToken", token)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        
        // ClaimsPrincipal principal;
        // Créez les propriétés du cookie
        
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };
        
        // try
        // {
        //     principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
        // }
        // catch (Exception ex)
        // {
        //     //! Voir pour une redirection vers le login
        //     return Unauthorized($"Token invalide: {ex.Message}");
        // }
        
        // //* Création d'une identité personalisée avec un claim contenant le token
        // var claims = new List<Claim>(principal.Claims)
        // {
        //     // Ajout d'une claim personnalisée "jwtToken"
        //     new Claim("jwtToken", token)
        // };

        // var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        // principal = new ClaimsPrincipal(identity);
        //
        // //* Émettre le cookie d'authentification
        // var authProperties = new AuthenticationProperties
        // {
        //     IsPersistent = true,
        //     ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
        // };

        // Connectez l'utilisateur en émettant le cookie d'authentification
        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, authProperties);

        return Redirect($"{_configuration["Url:Client"]}/?from=login");

        // encodedMessage = System.Net.WebUtility.UrlEncode(cryptedtoken);
        // return Redirect($"{_configuration["Url:Client"]}/?message={encodedMessage}");
        // return Redirect($"{_configuration["Url:Client"]}/dashboard/?message={encodedMessage}");
    }

    [HttpGet("signup")]
    public async Task<IActionResult> Signup()
    {
        string encodedMessage;
        
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded)
        {
            return Unauthorized();
        }
        
        //* Récupérer les informations de Google
        var sub = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
        var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
        var pictureUrl = result.Principal.FindFirst("urn:google:picture")?.Value;
        
        //* Je recherche si un user existe deja avec ce Username en BDD
        UserProfile? existingUser = await _userManager.FindByEmailAsync(email);
        
        //* Finir la gestion de l'erreur en front
        if (existingUser != null)
        {
            ErrorOAuth errorOAuth = new ErrorOAuth
            {
                Error = true,
                Message = "Cet utilisateur est déjà inscrit"
            };  
            
            var cryptedError = _serviceHashUrl.EncryptErrorOAuth(errorOAuth);
            
            encodedMessage = System.Net.WebUtility.UrlEncode(cryptedError);
            return Redirect($"{_configuration["Url:Client"]}/?error={encodedMessage}");
        }
        
        var newUser = new UserProfile
        {
            UserName = sub,
            Email = email,
            Name = name,
            Picture = pictureUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var saveResult = await _userManager.CreateAsync(newUser);

        if (!saveResult.Succeeded)
        {
            // Log ou retour d'erreurs
            foreach (var error in saveResult.Errors)
            {
                Console.WriteLine(error.Code + ": " + error.Description);
            }
            return BadRequest(saveResult.Errors);
            
        }
        
        //* Création du token et envoi vers le client
        var token = _jwtService.GenerateJwtToken(newUser);

        var cryptedtoken = _serviceHashUrl.EncryptErrorOAuth(token);
            
        encodedMessage = System.Net.WebUtility.UrlEncode(cryptedtoken);
        return Redirect($"{_configuration["Url:Client"]}/?message={encodedMessage}");
        // return Redirect($"{_configuration["Url:Client"]}/dashboard/?message={encodedMessage}");
    }
    
    [HttpGet("token")]
    [Authorize]
    public async Task<IActionResult> Token()
    {   
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            return Unauthorized();
        }
        
        var user = HttpContext.User;
        
        foreach (var claim in user.Claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
        }
        
        // Créez un objet UserProfile à partir des claims de l'utilisateur
        var userProfile = new UserProfile
        {
            UserName = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value,  // Récupérer le nom d'utilisateur
            Email = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value,  // Récupérer l'email
            Name = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value,  // Récupérer le nom complet
            Picture = user.FindFirst("urn:google:picture")?.Value,  // Récupérer la photo de profil (si elle est dans les claims)
            CreatedAt = null,  // Convertir la date de création
            UpdatedAt = null   // Convertir la date de mise à jour
        };
        
        
        // Claim Type: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier, Claim Value: 112987561183767145373
        // Claim Type: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name, Claim Value: Baptiste Boimard
        // Claim Type: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname, Claim Value: Baptiste
        // Claim Type: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname, Claim Value: Boimard
        // Claim Type: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress, Claim Value: bouketin27@gmail.com
        // Claim Type: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier, Claim Value: 112987561183767145373
        // Claim Type: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name, Claim Value: Baptiste Boimard
        // Claim Type: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress, Claim Value: bouketin27@gmail.com
        // Claim Type: urn:google:picture, Claim Value: https://lh3.googleusercontent.com/a/ACg8ocJE-fI4gV-e8_1nlt7lg-MWLZ0w6AoIVCNTtgwcKdjxIJ6xhA=s96-c

        // Générez un token JWT
        var token = _jwtService.GenerateJwtToken(userProfile);
    
        // Ajoutez le token JWT en tant que claim
        var claims = new List<Claim>
        {
            new Claim("jwtToken", token)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // Créez un cookie d'authentification
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };

        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, authProperties);
    
        // Redirigez l'utilisateur vers le frontend
        // return Redirect($"{_configuration["Url:Client"]}/?from=login");
        return Ok(token);

        // var token = User.FindFirst("jwtToken")?.Value;

        // if (token == null)
        // {
        //     return NotFound();
        // }
        //
        // return Ok(token);
    }
    
    [HttpGet("ping")]
    public IActionResult Ping()
    {   
        // //* On récupére le token depuis le CLaim Principal
        // var token = User.FindFirst("jwtToken")?.Value;
        //
        // if (token == null)
        // {
        //     return NotFound();
        // }
        
        // return Ok(token);
        return Ok();
    }
    
}
