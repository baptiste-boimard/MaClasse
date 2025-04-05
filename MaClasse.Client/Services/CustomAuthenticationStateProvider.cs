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
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public CustomAuthenticationStateProvider(
        IHttpContextAccessor httpContextAccessor,
        HttpClient httpClient,
        ProtectedLocalStorage protectedLocalStorage,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
        _protectedLocalStorage = protectedLocalStorage;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return await Task.FromResult(new AuthenticationState(_currentUser));
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

    // public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    // {
    //     var context = _httpContextAccessor.HttpContext;
    //     var sessionToken = context?.Request.Cookies["MaClasseAuth"];
    //
    //     if (string.IsNullOrEmpty(sessionToken))
    //         return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        
        // return await Task.FromResult(new AuthenticationState(_currentUser));

        //* Appel à la base de données de session
        //* Ajout des informations du user dans le UserState

        //     var session = await _sessionRepository.GetSessionByToken(sessionToken);
        //     if (session == null || session.Expiration < DateTime.UtcNow)
        //         return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        //
        //     var claims = new List<Claim>
        //     {
        //         new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()),
        //         new Claim(ClaimTypes.Role, session.Role),
        //         new Claim("session_token", session.Token)
        //     };
        //
        //     var identity = new ClaimsIdentity(claims, "session");
        //     var user = new ClaimsPrincipal(identity);
        //
        //     return new AuthenticationState(user);
    //     return null;
    // }
}