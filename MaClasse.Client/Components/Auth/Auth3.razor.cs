using MaClasse.Shared;
using Microsoft.AspNetCore.Components;

namespace MaClasse.Client.Components.Auth;

public partial class Auth3 : ComponentBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;

    public Auth3 (
        NavigationManager navigationManager, 
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor
    )
    {
        // var builder = new ConfigurationBuilder()
        //     .SetBasePath(Directory.GetCurrentDirectory())
        //     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //     .AddUserSecrets<Auth3>();
        //
        // IConfiguration configuration = builder.Build();
        // _googleClientId = configuration["Authentification:Google:ClientId"];
        // _googleClientSecret = configuration["Authentification:Google:ClientSecret"];
        
        _httpContextAccessor = httpContextAccessor;
        _navigationManager = navigationManager;
        _httpClient = httpClient;
    }

    private readonly string? _googleClientId;
    private readonly string? _googleClientSecret;

    private bool _isConnected = false;
    
    //* Variable de switch mode connexion ou inscription
    private bool _isLoginMode = true;
    //* Variable d'affiche du titre
    private string CurrentTitle => _isLoginMode ? "Bienvenue à nouveau" : "Créer un compte";
    //* Notre utilisateur
    private UserProfile? _profile;
    
    
    protected override async Task OnInitializedAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "api/profile");
        
        // Récupérer le cookie d'authentification depuis HttpContext
        var context = _httpContextAccessor.HttpContext;
        if (context != null && context.Request.Cookies.TryGetValue(".AspNetCore.Cookies", out var cookieValue))
        {
            // Ajoutez le cookie à l'en-tête de la requête
            request.Headers.Add("Cookie", $".AspNetCore.Cookies={cookieValue}");
        }

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<UserProfile>();
            _isConnected = true;
            _profile = content;

        }
        else
        {
            //! Faire un affichage de l'erreur
        }
    }

    // Fonction pour changer du mode
    private void ToggleMode()
    {
        _isLoginMode = !_isLoginMode;
    }

    private void ClickGoogleButton()
    {
        if (_isLoginMode)
        {
            
        }
        else
        {
            SignUpWithGoogle();
        }
        
    }
    
    private async void SignUpWithGoogle()
    {
        Nav.NavigateTo("https://localhost:7011/api/signup-google", forceLoad: true);
    }
}