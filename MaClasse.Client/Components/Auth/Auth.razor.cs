using System.Security.Claims;
using MaClasse.Client.Services;
using MaClasse.Shared.Models;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;

namespace MaClasse.Client.Components.Auth;

public partial class Auth : ComponentBase
{
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly ServiceHashUrl _serviceHashUrl;
    private readonly IConfiguration _configuration;
    private readonly IJSRuntime _jsRuntime;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ServiceAuthentication _serviceAuthentication;

    private readonly IHttpContextAccessor _httpContextAccessor;
    // private readonly HttpContext _httpContext;


    public Auth (
        NavigationManager navigationManager, 
        HttpClient httpClient,
        IDialogService dialogService,
        ServiceHashUrl serviceHashUrl,
        IConfiguration configuration,
        IJSRuntime jsRuntime,
        AuthenticationStateProvider authenticationStateProvider,
        ServiceAuthentication serviceAuthentication,
        IHttpContextAccessor httpContextAccessor)
        // HttpContext httpContext)
    {
        _navigationManager = navigationManager;
        _httpClient = httpClient;
        _dialogService = dialogService;
        _serviceHashUrl = serviceHashUrl;
        _configuration = configuration;
        _jsRuntime = jsRuntime;
        _authenticationStateProvider = authenticationStateProvider;
        _serviceAuthentication = serviceAuthentication;
        _httpContextAccessor = httpContextAccessor;
    }
    
    
    [Parameter] public EventCallback<string> OnTokenReceived { get; set; }
    private DotNetObjectReference<Auth>? dotNetRef;
    
    //* Variable d'affiche du titre
    private string _currentTitle = "Bienvenue sur votre application MaClasse";
    //* Variable pour la gestion du bouton Google
    private bool _isGoogleLogin = false;
    // //* Gestion des erreurs de login/inscription
    // private string _error = null!;
    // private bool _isError = false;
    // private string _isErrorMessage = null!;
    // private bool _isDialogOpened = false;
    
    // private async Task OpenDialogError()
    // {
    //     //* Paramètres à transmettre à la boîte de dialogue
    //     var parameters = new DialogParameters { { "Message", _isErrorMessage } };
    //         
    //     //* Options de la boîte de dialogue : fermeture sur Esc ou clic en dehors
    //     var options = new DialogOptions { CloseOnEscapeKey = true };
    //         
    //             
    //     //* Affichage de la boîte de dialogue
    //     var dialog = await _dialogService.ShowAsync<ErrorLoginDialog>("Erreur", parameters, options);
    //     
    //     var result = await dialog.Result; 
    //         
    //     if (result != null)
    //     {
    //         //* Après fermeture de la boîte, réinitialiser les variables d'erreur et de message
    //         _isError = false;
    //         _isErrorMessage = string.Empty;
    //         
    //         StateHasChanged();
    //         
    //         _navigationManager.NavigateTo($"{_configuration["Url:Client"]}", replace: true);
    //     }
    // }

    // //* Fonction pour changer du mode
    // private void ToggleMode()
    // {
    //     _isLoginMode = !_isLoginMode;
    // }
    

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var coucou = _configuration["Authentication:Google:ClientId"];
            // var module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", new object[] { "./js/google-signin.js" });
            dotNetRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("initializeGoogleLogin", dotNetRef, _configuration["Authentication:Google:ClientId"]);
        }
    }
    
    private async Task GoogleLoginAction()
    {
    
            

            // dotNetRef = DotNetObjectReference.Create(this);
            // await _jsRuntime.InvokeVoidAsync("initializeGoogleLogin", dotNetRef,  _configuration["Authentication:Google:ClientId"]);
            // await _httpContextAccessor.HttpContext.SignOutAsync();
            // _navigationManager.NavigateTo("https://accounts.google.com/logout", forceLoad: true);

            // await _jsRuntime.InvokeVoidAsync("google.accounts.id.disableAutoSelect");
           //! Supprime les cookies d'authentification du navigateur.
           var httpContext = _httpContextAccessor.HttpContext;
           //
           // await httpContext.SignOutAsync();

           var authStateProvider = (CustomAuthenticationStateProvider)_authenticationStateProvider;
           await authStateProvider.NotifyUserLogout();
           // Exemple de log pour l'utilisateur
           Console.WriteLine($"User après SignOut: {httpContext.User.Identity.IsAuthenticated}");

// Exemple de log pour les cookies
           foreach (var cookie in httpContext.Request.Cookies)
           {
               Console.WriteLine($"Cookie après SignOut: {cookie.Key} = {cookie.Value}");
           }

           // _navigationManager.NavigateTo("https://accounts.google.com/logout", forceLoad: true);
           await _jsRuntime.InvokeVoidAsync("google.accounts.id.disableAutoSelect");
            // await _jsRuntime.InvokeVoidAsync("googleaccounts");
            // _navigationManager.NavigateTo("/", forceLoad: true);

            // _navigationManager.NavigateTo("/"); // Rediriger vers la page d'accueil


    }
    
    [JSInvokable]
    public async Task ReceiveGoogleToken(string jwtToken)
    {
        _isGoogleLogin = true;
        StateHasChanged();

        var response = await _httpClient.PostAsJsonAsync(
            "https://localhost:7011/api/google-login", new GoogleTokenRequest{ Token = jwtToken });
        if (response.IsSuccessStatusCode)
        {
            //* Stock le token
            _serviceAuthentication.SetToken(jwtToken);
            _serviceAuthentication.AttachToken(_httpClient);
            
            var result = await response.Content.ReadFromJsonAsync<UserProfile>();
    
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result!.Id), // `sub` = ID Google
                new Claim(ClaimTypes.Email, result.Email), // `email`
                new Claim(ClaimTypes.Name, result.Name), // `name`
                new Claim(ClaimTypes.GivenName, result.GivenName), // `given_name`
                new Claim(ClaimTypes.Surname, result.FamilyName), // `family_name`
                new Claim("picture", result.Picture), // URL de la photo
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "MaClasse"));

            //* 🔥 Forcer Blazor à mettre à jour l'état d'authentification
            var authStateProvider = (CustomAuthenticationStateProvider)_authenticationStateProvider;
            await authStateProvider.NotifyUserAuthentication(principal);
            
            _navigationManager.NavigateTo("/dashboard");
        }
    }
}
