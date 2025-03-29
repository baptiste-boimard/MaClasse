using System.Security.Claims;
using MaClasse.Client.Services;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;


namespace Service.OAuth.Service;

public class UserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public UserService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }
    
    public async Task AuthenticateUser(UserProfile user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user!.Id),
            new Claim(ClaimTypes.Email, user!.Email), 
            new Claim(ClaimTypes.Name, user!.Name), 
            new Claim(ClaimTypes.Role, user!.Role), 
            new Claim(ClaimTypes.GivenName, user.GivenName),
            new Claim(ClaimTypes.Surname, user!.FamilyName),
            new Claim("picture", user!.Picture), 
            new Claim("createdAt", user!.CreatedAt.ToString()!), 
            new Claim("updatedAt", user!.UpdatedAt.ToString()!),
        };
        
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "MaClasse"));

        //* 🔥 Forcer Blazor à mettre à jour l'état d'authentification
        var authStateProvider = (CustomAuthenticationStateProvider)_authenticationStateProvider;
        await authStateProvider.NotifyUserAuthentication(principal);
    }
}