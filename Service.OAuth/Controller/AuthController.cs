using System.Security.Claims;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MaClasse.Api.Controllers;

[ApiController]
[Route("api")]
public class AuthController: ControllerBase
{
    private readonly UserManager<UserProfile> _userManager;

    public AuthController(UserManager<UserProfile> userManager)
    {
        _userManager = userManager;
    }
    
    [HttpGet("signin-google")]
    public IActionResult LoginGoogle(string returnUrl = "")
    {
        if (string.IsNullOrEmpty(returnUrl)) returnUrl = "https://localhost:7235";
        var properties = new AuthenticationProperties { RedirectUri = returnUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized();
        }

        //* Récupérer les informations de Google
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var pictureUrl = User.FindFirst("urn:google:picture")?.Value;

        var profile = new UserProfile
        {
            UserName = email,
            Email = email,
            Name = name,
            Picture = pictureUrl
        };
        
        return Ok(profile);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserProfile user)
    {
        var existingUser = await _userManager.FindByEmailAsync(user.UserName);
        
        //! Finir la gestion de l'erreur en front
        if (existingUser == null) return Conflict();

        var loginUser = new UserProfile
        {
            Id = existingUser.Id,
            UserName = user.UserName,
            Email = user.Email,
            Name = user.Name,
            Picture = user.Picture,
            CreatedAt = existingUser.CreatedAt,
            UpdatedAt = existingUser.UpdatedAt
        };

        return Ok(loginUser);
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] UserProfile user)
    {
        var existingUser = await _userManager.FindByEmailAsync(user.UserName);
        
        //! Finir la gestion de l'erreur en front
        if (existingUser != null) return Conflict();

        var newUser = new UserProfile
        {
            UserName = user.Email,
            Email = user.Email,
            Name = user.Name,
            Picture = user.Picture,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(newUser);

        if (!result.Succeeded)
        {
            // Log ou retour d'erreurs
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error.Code + ": " + error.Description);
            }
            return BadRequest(result.Errors);
            
        }

        return Ok(newUser);
    }
    
    //! Pour le front il faudrait que je stocke l'Id de l'utilisateur pour pouvoir le rajouter en head de requete
    
}
