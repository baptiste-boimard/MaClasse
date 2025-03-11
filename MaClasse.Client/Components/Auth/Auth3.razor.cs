using MaClasse.Client.Components.Errors;
using MaClasse.Client.Services;
using MaClasse.Shared.Models;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;


namespace MaClasse.Client.Components.Auth;

public partial class Auth3 : ComponentBase
{
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly ServiceHashUrl _serviceHashUrl;
    private readonly IConfiguration _configuration;
    private readonly ServiceAuthentication _authenticationService;
    // private readonly CustomAuthenticationStateProvider _authenticationStateProvider;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Auth3 (
        NavigationManager navigationManager, 
        HttpClient httpClient,
        IDialogService dialogService,
        ServiceHashUrl serviceHashUrl,
        IConfiguration configuration,
        ServiceAuthentication authenticationService,
        // CustomAuthenticationStateProvider authenticationStateProvider,
        AuthenticationStateProvider authStateProvider,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _navigationManager = navigationManager;
        _httpClient = httpClient;
        _dialogService = dialogService;
        _serviceHashUrl = serviceHashUrl;
        _configuration = configuration;
        _authenticationService = authenticationService;
        // _authenticationStateProvider = authenticationStateProvider;
        _authStateProvider = authStateProvider;
        _httpContextAccessor = httpContextAccessor;
    }
    
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

    protected override async Task OnInitializedAsync()
    {
        //* Récupérer l'URL complète
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        //* Utiliser QueryHelpers pour parser la query string
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
        
        if (query.TryGetValue("from", out var fromValue))
        {
            origin = fromValue;
            
            // if (origin == "login")
            // {
            //     _navigationManager.NavigateTo("/isauthenticated", forceLoad: true);
            // }
        

            var authState = await _authStateProvider.GetAuthenticationStateAsync();

            var coucou =  $"{_configuration["Url:ApiGateway"]}/api/auth/token";

            try
            {
                var cookieContainer = new System.Net.CookieContainer();
// Supposons que vous avez accès au cookie (ceci est difficile en Blazor Server car vous êtes sur le serveur)
// Vous devez récupérer le cookie de l'HttpContext si disponible :
                var httpContext = _httpContextAccessor.HttpContext; // Obtenez-le via IHttpContextAccessor
                if (httpContext.Request.Cookies.TryGetValue("MaClasseAuth", out var cookieValue))
                {
                    cookieContainer.Add(new Uri("https://localhost:7261"), new System.Net.Cookie("MaClasseAuth", cookieValue));
                }
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var response = await client.GetAsync("https://localhost:7261/api/auth/token");
                // var response =  await _httpClient.GetAsync($"{_configuration["Url:ApiGateway"]}/api/auth/token");

                // if (response == null)
                // {
                //     coucou = "df";
                // }
                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
            
                    if (token != null && !string.IsNullOrEmpty(token))
                    {
                        _authenticationService.SetToken(token);
                        _authenticationService.AttachToken(_httpClient);
                
                        await InvokeAsync(() => _navigationManager.NavigateTo("/dashboard"));
                    }
                }
                else
                {
                    _navigationManager.NavigateTo("/");
                }
                base.OnInitialized();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    
        // if (query.TryGetValue("message", out var stringMessage))
        // {
        //     var tokenMessage = stringMessage;
        //             
        //     _token = _serviceHashUrl.DecryptErrorOAuth(tokenMessage) as string;
        //         
        //     if (_token is string && !string.IsNullOrEmpty(_token))
        //     {
        //         //* Enregistrement du token dans le service d'authentification
        //         _authenticationService.SetToken(_token);
        //         //* Enregistrement du token dans l'HEAD du client http
        //         _authenticationService.AttachToken(_httpClient);
        //         //* Notifier que l'état d'authentification a changé
        //         _authenticationStateProvider.NotifyUserAuthentication();
        //         
        //         _navigationManager.NavigateTo($"{_configuration["Url:Client"]}/dashboard", replace: true);
        //     }
        // }    
        
        await base.OnInitializedAsync();
    }
 
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

            //* Obtenir l'URI absolu
            var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        
            //* Extraction des paramètres de la query string
            var query = QueryHelpers.ParseQuery(uri.Query);

            if (query.TryGetValue("error", out var stringParam))
            {
                _error = stringParam;
                    
                var errorOAuth = _serviceHashUrl.DecryptErrorOAuth(_error) as ErrorOAuth;
                _isError = errorOAuth.Error;
                _isErrorMessage = errorOAuth.Message;
            }
            
            if (_isError && !_isDialogOpened)
            {
                _isDialogOpened = true;

                await OpenDialogError();
            }
        }
    }

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

    private void GoogleLoginAction()
    {
        //* Lance le challenge Google pour connexion
        var returnUrl = $"{_configuration["Url:ApiGateway"]}/api/auth/login";
        var encodedReturnUrl = System.Net.WebUtility.UrlEncode(returnUrl);
        Nav.NavigateTo($"{_configuration["Url:ApiGateway"]}/api/auth/signin-google?returnUrl={encodedReturnUrl}", forceLoad: true);
    }

    private void GoogleSignupAction()
    {
        //* Lance le challenge Google pour inscription
        var returnUrl = $"{_configuration["Url:ApiGateway"]}/api/auth/signup";
        var encodedReturnUrl = System.Net.WebUtility.UrlEncode(returnUrl);
        Nav.NavigateTo($"{_configuration["Url:ApiGateway"]}/api/auth/signin-google?returnUrl={encodedReturnUrl}", forceLoad: true);
    }
}