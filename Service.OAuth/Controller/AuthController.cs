using System.Security.Claims;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.OAuth.Service;
using static Service.OAuth.Service.JwtService;

namespace Service.OAuth.Controller;

[ApiController]
[Route("api")]
public class AuthController: ControllerBase
{
    private readonly UserManager<UserProfile> _userManager;
    private readonly JwtService _jwtService;

    public AuthController(
        UserManager<UserProfile> userManager,
        JwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }
    
    [HttpGet("signin-google")]
    public IActionResult LoginGoogle(string returnUrl = "")
    {
        if (string.IsNullOrEmpty(returnUrl)) returnUrl = "https://localhost:7235";
        var properties = new AuthenticationProperties { RedirectUri = returnUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    // [HttpGet("profile")]
    // public IActionResult GetProfile()
    // {
    //     if (!User.Identity.IsAuthenticated)
    //     {
    //         return Unauthorized();
    //     }
    //
    //     //* Récupérer les informations de Google
    //     var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //     var email = User.FindFirst(ClaimTypes.Email)?.Value;
    //     var name = User.FindFirst(ClaimTypes.Name)?.Value;
    //     var pictureUrl = User.FindFirst("urn:google:picture")?.Value;
    //
    //     var profile = new UserProfile
    //     {
    //         UserName = sub,
    //         Email = email,
    //         Name = name,
    //         Picture = pictureUrl
    //     };
    //     
    //     //! Création d'un token et envoi de ce token dans la réponse pour qu'il le stocke dans
    //     //! le client
    //     
    //     return Ok(profile);
    // }

    [HttpGet("login")]
    public async Task<IActionResult> Login()
    {
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
        
        //! Finir la gestion de l'erreur en front
        if (existingUser == null)
        {
            return Redirect($"https://localhost:7235");
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

        //! Création du token
        var token = _jwtService.GenerateJwtToken(loginUser);
        
        var encodedToken = System.Net.WebUtility.UrlEncode(token);
        return Redirect($"https://localhost:7235/dashboard?token={encodedToken}");
    }

    [HttpGet("signup")]
    public async Task<IActionResult> Signup()
    {
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
        
        //! Finir la gestion de l'erreur en front
        if (existingUser != null)
        {
            return Redirect($"https://localhost:7235");
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
        
        //! Création du token
        var token = _jwtService.GenerateJwtToken(newUser);

        
        var encodedToken = System.Net.WebUtility.UrlEncode(token);
        return Redirect($"https://localhost:7235/dashboard?token={encodedToken}");
    }
    
    //! Pour le front il faudrait que je stocke l'Id de l'utilisateur pour pouvoir le rajouter en head de requete
    
}
