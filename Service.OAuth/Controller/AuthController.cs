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
        
        //* Finir la gestion de l'erreur en front
        if (existingUser == null)
        {
            var error = true;
            var message = "Cet utilisateur n'est pas encore inscrit";
            var encodedMessage = System.Net.WebUtility.UrlEncode(message);
            return Redirect($"https://localhost:7235/?error={error}&message={encodedMessage}");
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

        //* Création du token et envoie vers le client
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
        
        //* Finir la gestion de l'erreur en front
        if (existingUser != null)
        {
            var error = true;
            var message = "Un utilisateur avec cette email existe déjà";
            var encodedMessage = System.Net.WebUtility.UrlEncode(message);
            return Redirect($"https://localhost:7235/?error={error}&message={encodedMessage}");
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

        
        var encodedToken = System.Net.WebUtility.UrlEncode(token);
        return Redirect($"https://localhost:7235/dashboard?token={encodedToken}");
    }
}
