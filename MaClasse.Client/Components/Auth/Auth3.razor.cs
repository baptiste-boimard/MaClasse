using MaClasse.Shared;
using MaClasse.Shared.Models;
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
        _httpContextAccessor = httpContextAccessor;
        _navigationManager = navigationManager;
        _httpClient = httpClient;
    }
    
    private bool _isConnected = false;
    
    //* Variable de switch mode connexion ou inscription
    private bool _isLoginMode = true;
    //* Variable d'affiche du titre
    private string CurrentTitle => _isLoginMode ? "Bienvenue à nouveau" : "Créer un compte";
    //* Notre utilisateur
    private UserProfile? _profile;
    
    
    protected override async Task OnInitializedAsync()
    {
        // //* Vérifier si le paramètre fromSignup est présent dans l'URL
        // var uri = Nav.ToAbsoluteUri(Nav.Uri);
        // var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
        //
        // if (query.TryGetValue("fromSignup", out var fromSignupValue) && fromSignupValue == "true")
        // {
        //     //* Appel à l'endpoint pour récupérer le profil
        //     var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/profile");
        //     
        //     //* Récupérer le cookie d'authentification depuis HttpContext
        //     //! il semble que je ne sois pas obligé de passer ce cookie
        //     var context = _httpContextAccessor.HttpContext;
        //     if (context != null && context.Request.Cookies.TryGetValue(".AspNetCore.Cookies", out var cookieValue))
        //     {
        //         //* Ajoutez le cookie à l'en-tête de mon client
        //         _httpClient.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.Cookies={cookieValue}");
        //     
        //     }
        //     
        //     var response = await _httpClient.SendAsync(request);
        //     
        //     if (response.IsSuccessStatusCode)
        //     {
        //         UserProfile userProfile = await response.Content.ReadFromJsonAsync<UserProfile>();
        //         
        //         //! Faire apparaitre une modal pour le completer le profil
        //         //! puis envoyer toutes les données au back
        //         
        //         var content = JsonContent.Create(userProfile);
        //
        //         var signupResponse = await _httpClient.PostAsync("api/auth/signup", content);
        //
        //         if (signupResponse.IsSuccessStatusCode)
        //         {
        //             Nav.NavigateTo("/dashboard");
        //         }
        //         else
        //         {
        //             //! Faire un affichage de l'erreur comme quoi je ne récuépère pas les infos de Google
        //         }
        //     }
        // }
        //
        // if (fromSignupValue == "false")
        // {
        //     //* Appel à l'endpoint pour récupérer le profil
        //     var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/profile");
        //     
        //     //* Récupérer le cookie d'authentification depuis HttpContext
        //     var context = _httpContextAccessor.HttpContext;
        //     if (context != null && context.Request.Cookies.TryGetValue(".AspNetCore.Cookies", out var cookieValue))
        //     {
        //         //* Ajoutez le cookie à l'en-tête de mon client
        //         _httpClient.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.Cookies={cookieValue}");
        //     
        //     }
        //     
        //     var response = await _httpClient.SendAsync(request);
        //     
        //     if (response.IsSuccessStatusCode)
        //     {
        //         UserProfile userProfile = await response.Content.ReadFromJsonAsync<UserProfile>();
        //         
        //         var content = JsonContent.Create(userProfile);
        //
        //         var signupResponse = await _httpClient.PostAsync("api/auth/login", content);
        //
        //         if (signupResponse.IsSuccessStatusCode)
        //         {
        //             Nav.NavigateTo("/dashboard");
        //         }
        //         else
        //         {
        //             //! Faire un affichage de l'erreur comme quoi je ne récuépère pas les infos de Google
        //         }
        //     }
        // }
    }

    // Fonction pour changer du mode
    private void ToggleMode()
    {
        _isLoginMode = !_isLoginMode;
    }

    private void GoogleLoginAction()
    {
        //* Lance le challenge Google pour connexion
        //* Le returnUrl ici redirige directement vers le dashboard après authentification
        //! Je choisis l'url de redirection vers mon api
        var returnUrl = "https://localhost:7261/api/auth/login";
        var encodedReturnUrl = System.Net.WebUtility.UrlEncode(returnUrl);
        Nav.NavigateTo($"https://localhost:7261/api/auth/signin-google?returnUrl={encodedReturnUrl}", forceLoad: true);
    }

    private void GoogleSignupAction()
    {
        //* Lance le challenge Google pour inscription
        //* Le returnUrl redirige vers une page de callback dédiée ou le composant peut déclencher une récupération du profil
        var returnUrl = "https://localhost:7261/api/auth/signup";
        var encodedReturnUrl = System.Net.WebUtility.UrlEncode(returnUrl);
        Nav.NavigateTo($"https://localhost:7261/api/auth/signin-google?returnUrl={encodedReturnUrl}", forceLoad: true);
    }
    
    // private async void SignUpWithGoogle()
    // {
    //     Nav.NavigateTo("https://localhost:7261/api/auth/signup-google", forceLoad: true);
    // }
}