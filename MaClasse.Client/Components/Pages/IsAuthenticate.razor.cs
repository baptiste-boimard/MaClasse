using MaClasse.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MaClasse.Client.Components.Pages;

public partial class IsAuthenticate : ComponentBase
{
    private readonly HttpClient _httpClient;
    private readonly ServiceAuthentication _authenticationService;
    private readonly NavigationManager _navigationManager;
    private readonly IConfiguration _configuration;
    private readonly AuthenticationStateProvider _authStateProvider;

    public IsAuthenticate(
        HttpClient httpClient,
        ServiceAuthentication authenticationService,
        NavigationManager navigationManager,
        IConfiguration configuration,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _authenticationService = authenticationService;
        _navigationManager = navigationManager;
        _configuration = configuration;
        _authStateProvider = authStateProvider;
    }
    
    //* Gestion de l'origine de la redirection
    private string origin;

    protected override async Task OnInitializedAsync()
    {
        // // Récupérer l'URL actuelle
        // var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        // // Parser les paramètres de la query string
        // var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
        //
        // if (queryParams.TryGetValue("from", out var fromValue))
        // {
        //     origin = fromValue;
        // }
        //
        // // Vous pouvez ensuite adapter le comportement selon l'origine :
        // if (origin == "login")
        // {
        //     _navigationManager.NavigateTo("/isauthenticated", forceLoad: true);
        // }
        

        var authState = await _authStateProvider.GetAuthenticationStateAsync();

        var coucou =  $"{_configuration["Url:ApiGateway"]}/api/auth/token";

        // try
        // {
        //     var response = await _httpClient.GetAsync("/api/auth/ping");
        //     Console.WriteLine("coucou");
        //     if (response.IsSuccessStatusCode)
        //     {
        //         var token = await response.Content.ReadAsStringAsync();
        //     
        //         if (token != null && !string.IsNullOrEmpty(token))
        //         {
        //             _authenticationService.SetToken(token);
        //             _authenticationService.AttachToken(_httpClient);
        //         
        //             _navigationManager.NavigateTo("/dashboard");
        //         }
        //     }
        //     else
        //     {
        //         _navigationManager.NavigateTo("/");
        //     }
        //     base.OnInitialized();
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine(e);
        //     throw;
        // }
        
        try
        {
            var handler = new HttpClientHandler
            {
                // Pour le développement : accepter tout certificat (à n'utiliser qu'en dev)
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            using var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
    
            Console.WriteLine("Début de l'appel GET");
            var response = await client.GetAsync("https://localhost:7011/api/ping");
            Console.WriteLine("Appel terminé");
            Console.WriteLine($"Status Code : {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception capturée : {ex.Message}");
        }

    }
}