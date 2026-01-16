using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MaClasse.Client.States;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MaClasse.Client.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
    
    public CustomAuthenticationStateProvider(
        ProtectedLocalStorage protectedLocalStorage)
    {
        _protectedLocalStorage = protectedLocalStorage;
    }
    
    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var result = await _protectedLocalStorage.GetAsync<string>("MaClasseAuth");

            if (result.Success && !string.IsNullOrEmpty(result.Value))
            {
                return new AuthenticationState(_currentUser);
            }
        }
        catch (InvalidOperationException)
        {
            //* On ne fait, rien cela laisse le temps au render de ce faire pour récup le protectedLocalStorage
        }
        
        return new AuthenticationState(_currentUser);
    }

    public Task NotifyUserAuthentication(ClaimsPrincipal user)
    {
        _currentUser = user;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        return Task.CompletedTask;
    }

    public Task NotifyUserLogout()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        return Task.CompletedTask;
    }
}