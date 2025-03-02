using System.Security.Claims;
using MaClasse.Shared;
using MaClasse.Shared.Models;
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

        //* Récupérer les informations de Google
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
    
    //! Nouvelle méthode pour récupérer les infos depuis le front avec les infos de l'utilisateur et les infos
    //! de personnalisation de profil avec un POST
    
    //! Pareil pour le login,on récupère les infos depuis le back et le front demande s'il peut se connecter
    //! avec un POST
    
    //! Pour le front il faudrait que je stocke l'Id de l'utilisateur pour pouvoir le rajouter en head de requete
    
}
