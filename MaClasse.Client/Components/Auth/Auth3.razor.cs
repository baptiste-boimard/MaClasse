using System.Net;
using System.Security.Claims;
using MaClasse.Client.Components.Errors;
using MaClasse.Client.Services;
using MaClasse.Shared.Models;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
using MudBlazor;
using Service.OAuth.Controller;


namespace MaClasse.Client.Components.Auth;

public partial class Auth3 : ComponentBase
{
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly ServiceHashUrl _serviceHashUrl;
    private readonly IConfiguration _configuration;

    private readonly IJSRuntime _jsRuntime;

    private readonly AuthenticationStateProvider _authenticationStateProvider;
    // private readonly IHttpContextAccessor _httpContextAccessor;
    // private readonly CustomAuthenticationStateProvider _customAuthenticationStateProvider;

    public Auth3 (
        NavigationManager navigationManager, 
        HttpClient httpClient,
        IDialogService dialogService,
        ServiceHashUrl serviceHashUrl,
        IConfiguration configuration,
        IJSRuntime jsRuntime,
        AuthenticationStateProvider authenticationStateProvider
        // IHttpContextAccessor httpContextAccessor
        // CustomAuthenticationStateProvider customAuthenticationStateProvider
    )
    {
        _navigationManager = navigationManager;
        _httpClient = httpClient;
        _dialogService = dialogService;
        _serviceHashUrl = serviceHashUrl;
        _configuration = configuration;
        _jsRuntime = jsRuntime;
        _authenticationStateProvider = authenticationStateProvider;
        // _httpContextAccessor = httpContextAccessor;
        // _customAuthenticationStateProvider = customAuthenticationStateProvider;
    }
    
    
    [Parameter] public EventCallback<string> OnTokenReceived { get; set; }
    private const string GoogleClientId = "419056052171-4stscg6up8etnu68m5clp4gi0m3im8ea.apps.googleusercontent.com";
    private DotNetObjectReference<Auth3>? dotNetRef;
    
    
    
    private bool _isConnected = false;
    
    //* Variable de switch mode connexion ou inscription
    private bool _isLoginMode = true;
    //* Variable d'affiche du titre
    private string CurrentTitle => _isLoginMode ? "Bienvenue à nouveau" : "Créer un compte";
    //* Notre utilisateur
    private UserProfile? _profile;
    //* Gestion des erreurs de login/inscription
    private string _error;
    private bool _isError = false;
    private string _isErrorMessage;
    private bool _isDialogOpened = false;
    //* Token d'authentification
    private string? _token;

    private string origin;

    // protected override async Task OnInitializedAsync()
    // {
        
        // var authState = await _authStateProvider.GetAuthenticationStateAsync();
        // var user = authState.User;IHttpContextAccessor
        
        
        // //* Récupérer l'URL complète
        // var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        // //* Utiliser QueryHelpers pour parser la query string
        // var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
        //
        // if (query.TryGetValue("from", out var fromValue))
        // {
        //     origin = fromValue;
        //     
        //     // if (origin == "login")
        //     // {
        //     //     _navigationManager.NavigateTo("/isauthenticated", forceLoad: true);
        //     // }
        //
        //
        //     // var authState = await _authStateProvider.GetAuthenticationStateAsync();
        //     //
        //     // var coucou =  $"{_configuration["Url:ApiGateway"]}/api/auth/token";
        //
        //     try
        //     {
        //         var cookieContainer = new System.Net.CookieContainer();
        //         // Supposons que vous avez accès au cookie (ceci est difficile en Blazor Server car vous êtes sur le serveur)
        //         // Vous devez récupérer le cookie de l'HttpContext si disponible :
        //         var httpContext = _httpContextAccessor.HttpContext; // Obtenez-le via IHttpContextAccessor
        //         if (httpContext.Request.Cookies.TryGetValue("MaClasseAuth", out var cookieValue))
        //         {
        //             cookieContainer.Add(new Uri("https://localhost:7261"), new System.Net.Cookie("MaClasseAuth", cookieValue));
        //         }
        //         var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
        //         var client = new HttpClient(handler);
        //         var response = await client.GetAsync("https://localhost:7261/api/auth/token");
        //         // var response =  await _httpClient.GetAsync($"{_configuration["Url:ApiGateway"]}/api/auth/token");
        //
        //         // if (response == null)
        //         // {
        //         //     coucou = "df";
        //         // }
        //         if (response.IsSuccessStatusCode)
        //         {
        //             var token = await response.Content.ReadAsStringAsync();
        //     
        //             if (token != null && !string.IsNullOrEmpty(token))
        //             {
        //                 _authenticationService.SetToken(token);
        //                 _authenticationService.AttachToken(_httpClient);
        //         
        //                 await InvokeAsync(() => _navigationManager.NavigateTo("/dashboard"));
        //             }
        //         }
        //         else
        //         {
        //             _navigationManager.NavigateTo("/");
        //         }
        //         base.OnInitialized();
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //         throw;
        //     }
        // }
        //
        // // if (query.TryGetValue("message", out var stringMessage))
        // // {
        // //     var tokenMessage = stringMessage;
        // //             
        // //     _token = _serviceHashUrl.DecryptErrorOAuth(tokenMessage) as string;
        // //         
        // //     if (_token is string && !string.IsNullOrEmpty(_token))
        // //     {
        // //         //* Enregistrement du token dans le service d'authentification
        // //         _authenticationService.SetToken(_token);
        // //         //* Enregistrement du token dans l'HEAD du client http
        // //         _authenticationService.AttachToken(_httpClient);
        // //         //* Notifier que l'état d'authentification a changé
        // //         _authenticationStateProvider.NotifyUserAuthentication();
        // //         
        // //         _navigationManager.NavigateTo($"{_configuration["Url:Client"]}/dashboard", replace: true);
        // //     }
        // // }    
        //
        // await base.OnInitializedAsync();
    // }
 
    
    // protected override async Task OnAfterRenderAsync(bool firstRender)
    // {
    //     if (firstRender)
    //     {
    //
    //         //* Obtenir l'URI absolu
    //         var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
    //     
    //         //* Extraction des paramètres de la query string
    //         var query = QueryHelpers.ParseQuery(uri.Query);
    //
    //         if (query.TryGetValue("error", out var stringParam))
    //         {
    //             _error = stringParam;
    //                 
    //             var errorOAuth = _serviceHashUrl.DecryptErrorOAuth(_error) as ErrorOAuth;
    //             _isError = errorOAuth.Error;
    //             _isErrorMessage = errorOAuth.Message;
    //         }
    //         
    //         if (_isError && !_isDialogOpened)
    //         {
    //             _isDialogOpened = true;
    //
    //             await OpenDialogError();
    //         }
    //     }
    // }

    private async Task OpenDialogError()
    {
        //* Paramètres à transmettre à la boîte de dialogue
        var parameters = new DialogParameters { { "Message", _isErrorMessage } };
            
        //* Options de la boîte de dialogue : fermeture sur Esc ou clic en dehors
        var options = new DialogOptions { CloseOnEscapeKey = true };
            
                
        //* Affichage de la boîte de dialogue
        var dialog = await _dialogService.ShowAsync<ErrorLoginDialog>("Erreur", parameters, options);
        
        var result = await dialog.Result; 
            
        if (result != null)
        {
            //* Après fermeture de la boîte, réinitialiser les variables d'erreur et de message
            _isError = false;
            _isErrorMessage = string.Empty;
            
            StateHasChanged();
            
            _navigationManager.NavigateTo($"{_configuration["Url:Client"]}", replace: true);


        }
    }

    //* Fonction pour changer du mode
    private void ToggleMode()
    {
        _isLoginMode = !_isLoginMode;
    }

    private async Task GoogleLoginAction()
    {
        // //* Lance le challenge Google pour connexion
        // var returnUrl = $"{_configuration["Url:ApiGateway"]}/api/auth/login";
        // var encodedReturnUrl = System.Net.WebUtility.UrlEncode(returnUrl);
        // Nav.NavigateTo($"{_configuration["Url:ApiGateway"]}/api/auth/signin-google?returnUrl={encodedReturnUrl}", forceLoad: true);
        
        dotNetRef = DotNetObjectReference.Create(this);
        await _jsRuntime.InvokeVoidAsync("initializeGoogeLogin", dotNetRef, GoogleClientId);
    }

    private void GoogleSignupAction()
    {
        // //* Lance le challenge Google pour inscription
        // var returnUrl = $"{_configuration["Url:ApiGateway"]}/api/auth/signup";
        // var encodedReturnUrl = System.Net.WebUtility.UrlEncode(returnUrl);
        // Nav.NavigateTo($"{_configuration["Url:ApiGateway"]}/api/auth/signin-google?returnUrl={encodedReturnUrl}", forceLoad: true);
    }
    
    [JSInvokable]
    public async Task ReceiveGoogleToken(string jwtToken)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "https://localhost:7011/api/google-login", new GoogleTokenRequest{ Token = jwtToken });
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResult>();
          
            // Créer une authentification via appel à un endpoint interne Blazor Server
            // var loginResponse = await _httpClient.PostAsJsonAsync(
            //     "https://localhost:7235/login-with-email", new LoginResult
            //     {
            //         Id = result.Id,
            //         Email = result.Email,
            //         Name = result.Name,
            //         GivenName = result.GivenName,
            //         FamilyName = result.FamilyName,
            //         Picture = result.Picture
            //     });
            //
            // if (loginResponse.IsSuccessStatusCode)
            //
            // {
                // // 🔥 Forcer Blazor à mettre à jour l'état d'authentification
                // var authStateProvider = (CustomAuthenticationStateProvider)_authStateProvider;
                // await authStateProvider.NotifyUserAuthentication(new ClaimsPrincipal(new ClaimsIdentity(new[]
                // {
                //     // new Claim(ClaimTypes.Name, "Utilisateur Test"),
                //     // new Claim(ClaimTypes.Email, "test@example.com"),
                //     new Claim(ClaimTypes.Name, result.Email),
                //     new Claim(ClaimTypes.Email, authStateProvider.GetAuthenticationStateAsync().Result.User.Claims.ToString())
                // }, "CustomAuth")));
                //
                //
                // _navigationManager.NavigateTo("/dashboard");
                
                
                // 🔥 Forcer Blazor à mettre à jour l'état d'authentification
                var authStateProvider = (CustomAuthenticationStateProvider)_authStateProvider;
    
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.Id), // `sub` = ID Google
                    new Claim(ClaimTypes.Email, result.Email), // `email`
                    new Claim(ClaimTypes.Name, result.Name), // `name`
                    new Claim(ClaimTypes.GivenName, result.GivenName), // `given_name`
                    new Claim(ClaimTypes.Surname, result.FamilyName), // `family_name`
                    new Claim("picture", result.Picture), // URL de la photo
                };

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));

                await authStateProvider.NotifyUserAuthentication(principal);

                _navigationManager.NavigateTo("/dashboard");
            // }
        //     else
        //         Console.WriteLine("Erreur création cookie");
        //
        }
    }
    
    
    [Inject] private AuthenticationStateProvider _authStateProvider { get; set; }
    
    public class LoginResult
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Picture { get; set; }
    }

}
