using System.Security.Claims;
using MaClasse.Client.Services;
using MaClasse.Shared.Models;
using MaClasse.Shared.Service;
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
    //* Retour de la reponse d'Auth
    private AuthReturn? returnResponse;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            dotNetRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync(
                "initializeGoogleLogin",
                dotNetRef,
                _configuration["Authentication:Google:ClientId"]);
        }
    }
    
    [JSInvokable]
    public async Task ReceiveGoogleToken(string jwtToken)
    {
        _isGoogleLogin = true;
        StateHasChanged();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/auth/google-login",
            new GoogleTokenRequest{ Token = jwtToken });
        
        if (response.IsSuccessStatusCode)
        {
            //* Stock le token
            _serviceAuthentication.SetToken(jwtToken);
            _serviceAuthentication.AttachToken(_httpClient);
            
            returnResponse = await response.Content.ReadFromJsonAsync<AuthReturn>();
    
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, returnResponse!.User!.Id), // `sub` = ID Google
                new Claim(ClaimTypes.Email, returnResponse!.User!.Email), // `email`
                new Claim(ClaimTypes.Name, returnResponse!.User!.Name), // `name`
                new Claim(ClaimTypes.GivenName, returnResponse!.User!.GivenName), // `given_name`
                new Claim(ClaimTypes.Surname, returnResponse!.User!.FamilyName), // `family_name`
                new Claim("picture", returnResponse!.User!.Picture), // URL de la photo
                new Claim("createdAt", returnResponse!.User!.CreatedAt.ToString()!), 
                new Claim("updatedAt", returnResponse!.User!.UpdatedAt.ToString()!),
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "MaClasse"));

            //* 🔥 Forcer Blazor à mettre à jour l'état d'authentification
            var authStateProvider = (CustomAuthenticationStateProvider)_authenticationStateProvider;
            await authStateProvider.NotifyUserAuthentication(principal);

            //* Recherche si c'est un nouvel utilisateur, dans ce cas on ouvre la modal de complément d'infos
            if (returnResponse.IsNewUser)
            {
                await OpenDialogAuth(returnResponse.User);
            }
            else
            {
                _navigationManager.NavigateTo("/dashboard");
            }
            
        }
    }
    
    private async Task OpenDialogAuth(UserProfile user)
    {
        //* Paramètres à transmettre à la boîte de dialogue
        var parameters = new DialogParameters { { "user", user } };
            
        //* Options de la boîte de dialogue : fermeture sur Esc ou clic en dehors
        var options = new DialogOptions
        {
            CloseOnEscapeKey = false,
            CloseButton = false,
            FullWidth = true,          // design
            MaxWidth = MaxWidth.Small, // design
        };
            
                
        //* Affichage de la boîte de dialogue
        var dialog = await _dialogService.ShowAsync<AuthDialog>("", parameters, options);
        
        var result = await dialog.Result; 
        
        if (!result.Canceled && result.Data is string role && !string.IsNullOrWhiteSpace(role))
        {
            var payload = new { Role = role };
            //* Requete vers le back pour compléter le profil
            var response = await _httpClient.PostAsJsonAsync(
                $"{_configuration["Url:ApiGateway"]}/api/auth/finished-signup", payload);

            if (response.IsSuccessStatusCode)
            {
                var coucouc = "coucou";
                _navigationManager.NavigateTo("/dashboard");
            }
            
        }
        else
        {
            await OpenDialogAuth(user); // 💥 relance si fermé sans valider
        }

    }
}
