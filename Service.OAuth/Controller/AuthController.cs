using System.Security.Claims;
using MaClasse.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace MaClasse.Api.Controllers;

[ApiController]
[Route("api")]
public class AuthController: ControllerBase
{
    [HttpGet("signup-google")]
    public IActionResult LoginGoogle(string returnUrl = "https://localhost:7235")
    {
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

        // Récupérer les informations
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var pictureUrl = User.FindFirst("urn:google:picture")?.Value;

        // À partir de là, vous pouvez enregistrer l'utilisateur dans votre BDD
        // ou retourner ces informations au front-end, etc.
        return Ok(new UserProfile
        {
            Email = email,
            Name = name,
            Picture = pictureUrl
        });
    }
}