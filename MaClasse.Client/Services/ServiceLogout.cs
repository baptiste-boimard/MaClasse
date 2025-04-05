using MaClasse.Client.States;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MaClasse.Client.Services;

public class ServiceLogout
{
    private readonly IConfiguration _configuration;
    private readonly UserState _userState;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly NavigationManager _navigationManager;
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly HttpClient _httpClient;

    public ServiceLogout(
        HttpClient httpClient,
        IConfiguration configuration,
        UserState userState,
        AuthenticationStateProvider authenticationStateProvider,
        NavigationManager navigationManager,
        ProtectedLocalStorage protectedLocalStorage)
    {
        _configuration = configuration;
        _userState = userState;
        _authenticationStateProvider = authenticationStateProvider;
        _navigationManager = navigationManager;
        _protectedLocalStorage = protectedLocalStorage;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async void Logout(string idSession)
    {
        //* J'efface la session avec un appel api
        var payload = JsonContent.Create(new { IdSession = idSession });

        var response = await _httpClient.PostAsync($"{_configuration["Url:ApiGateway"]}/api/auth/logout-session", payload);
        
        //* J'efface le UserState
        _userState.ResetUserState();
        
        //* J'efface le user des identity
        //* 🔥 Forcer Blazor à mettre à jour l'état d'authentification
        var authStateProvider = (CustomAuthenticationStateProvider)_authenticationStateProvider;
        await authStateProvider.NotifyUserLogout();
        
        //* J'efface le token du localstorage
        await _protectedLocalStorage.DeleteAsync("MaClasseAuth");
        
        if (response.IsSuccessStatusCode)
        {
            _navigationManager.NavigateTo("/");
        }
    }
}