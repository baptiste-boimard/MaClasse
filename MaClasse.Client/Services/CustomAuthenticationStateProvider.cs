using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace MaClasse.Client.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ServiceAuthentication _serviceAuthentication;

    public CustomAuthenticationStateProvider(ServiceAuthentication serviceAuthentication)
    {
        _serviceAuthentication = serviceAuthentication;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
       var token = _serviceAuthentication.JwtToken;
       ClaimsPrincipal principal;

       if (!string.IsNullOrEmpty(token))
       {
           try
           {
               //* Utiliser JwtSecurityTokenHandler pour décoder le token
               var handler = new JwtSecurityTokenHandler();
               var jwtToken = handler.ReadJwtToken(token);
               
               //* Construire une identité avec les claims contenus dans le token
               var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
               principal = new ClaimsPrincipal(identity);
           }
           catch
           {
               //* En cas d'erreur de décodage, on considère que l'utilisateur n'est pas authentifié
               principal = new ClaimsPrincipal(new ClaimsIdentity());
           }
       }
       else
       {
              principal = new ClaimsPrincipal(new ClaimsIdentity());
       }
       
       return await Task.FromResult(new AuthenticationState(principal));
    }
    
    public void NotifyUserAuthentication()
    {
        var authState = GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(authState);
    }
}