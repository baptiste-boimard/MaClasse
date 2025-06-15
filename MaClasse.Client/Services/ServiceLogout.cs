using MaClasse.Client.States;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MaClasse.Client.Services;

public class ServiceLogout
{
    private readonly IConfiguration _configuration;
    private readonly UserState _userState;
    private readonly SchedulerState _schedulerState;
    private readonly ViewDashboardState _viewDashboardState;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly NavigationManager _navigationManager;
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly LessonState _lessonState;
    private readonly HttpClient _httpClient;

    public ServiceLogout(
        HttpClient httpClient,
        IConfiguration configuration,
        UserState userState,
        SchedulerState schedulerState,
        ViewDashboardState viewDashboardState,
        AuthenticationStateProvider authenticationStateProvider,
        NavigationManager navigationManager,
        ProtectedLocalStorage protectedLocalStorage,
        LessonState lessonState)
    {
        _configuration = configuration;
        _userState = userState;
        _schedulerState = schedulerState;
        _viewDashboardState = viewDashboardState;
        _authenticationStateProvider = authenticationStateProvider;
        _navigationManager = navigationManager;
        _protectedLocalStorage = protectedLocalStorage;
        _lessonState = lessonState;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async void Logout(string idSession)
    {
        //* J'efface la session avec un appel api
        var payload = JsonContent.Create(new { IdSession = idSession });

        var response = await _httpClient.PostAsync($"{_configuration["Url:ApiGateway"]}/api/auth/logout-session", payload);
        
        //* J'efface les States
        _userState.ResetUserState();
        _viewDashboardState.ResetViewDashboardState();
        _schedulerState.ResetSchedulerState();
        _lessonState.ResetLessonState();
        
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