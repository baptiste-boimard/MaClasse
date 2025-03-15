using System.Net.Http.Headers;
using MaClasse.Client.Services;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace MaClasse.Client.Components.DashboardContent;

public partial class DashboardContent : ComponentBase
{
    private readonly HttpClient _httpClient;

    private readonly AuthenticationStateProvider _authenticationStateProvider;
    // private readonly NavigationManager _navigationManager;
    // private readonly ServiceHashUrl _serviceHashUrl;
    // private readonly ServiceAuthentication _authenticationService;
    // private readonly CustomAuthenticationStateProvider _authenticationStateProvider;

    public DashboardContent(
        HttpClient httpClient,
        AuthenticationStateProvider authenticationStateProvider
        // NavigationManager navigationManager,
        // ServiceHashUrl serviceHashUrl,
        // ServiceAuthentication authenticationService,
        // CustomAuthenticationStateProvider authenticationStateProvider
        )
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
        // _navigationManager = navigationManager;
        // _serviceHashUrl = serviceHashUrl;
        // _authenticationService = authenticationService;
        // _authenticationStateProvider = authenticationStateProvider;
    }

    private string? _token;
    
    protected override async Task OnInitializedAsync()
    {
        
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        
        var response = await _httpClient.GetAsync("https://localhost:7261/api/auth/whoami");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
        else
        {
            Console.WriteLine("Error");
        }
    //     //* Récupérer l'URL complète
    //     var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
    //     //* Utiliser QueryHelpers pour parser la query string
    //     var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
    //
    //     if (query.TryGetValue("message", out var stringMessage))
    //     {
    //         var tokenMessage = stringMessage;
    //                 
    //         _token = _serviceHashUrl.DecryptErrorOAuth(tokenMessage) as string;
    //             
    //         if (_token is string && !string.IsNullOrEmpty(_token))
    //         {
    //             //* Enregistrement du token dans le service d'authentification
    //             _authenticationService.SetToken(_token);
    //             //* Enregistrement du token dans l'HEAD du client http
    //             _authenticationService.AttachToken(_httpClient);
    //             //* Notifier que l'état d'authentification a changé
    //             _authenticationStateProvider.NotifyUserAuthentication();
    //         }
    //     }    
    //     
        await base.OnInitializedAsync();
    }
}