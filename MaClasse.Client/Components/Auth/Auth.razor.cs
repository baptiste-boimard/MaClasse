using System.Security.Claims;
using MaClasse.Client.Services;
using MaClasse.Client.States;
using MaClasse.Shared.Models;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Service.OAuth.Service;

namespace MaClasse.Client.Components.Auth;

public partial class Auth : ComponentBase
{
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly ServiceHashUrl _serviceHashUrl;
    private readonly IConfiguration _configuration;
    private readonly IJSRuntime _jsRuntime;
    private readonly ServiceAuthentication _serviceAuthentication;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserService _userService;
    private readonly UserState _userState;


    public Auth (
        NavigationManager navigationManager, 
        HttpClient httpClient,
        IDialogService dialogService,
        ServiceHashUrl serviceHashUrl,
        IConfiguration configuration,
        IJSRuntime jsRuntime,
        ServiceAuthentication serviceAuthentication,
        IHttpContextAccessor httpContextAccessor,
        UserService userService,
        UserState userState)
    {
        _navigationManager = navigationManager;
        _httpClient = httpClient;
        _dialogService = dialogService;
        _serviceHashUrl = serviceHashUrl;
        _configuration = configuration;
        _jsRuntime = jsRuntime;
        _serviceAuthentication = serviceAuthentication;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _userState = userState;
    }
    
    [Parameter] public EventCallback<string> OnTokenReceived { get; set; }
    private DotNetObjectReference<Auth>? dotNetRef;
    
    //* Variable d'affiche du titre
    private string _currentTitle = "Bienvenue sur votre application MaClasse";
    //* Variable pour la gestion du bouton Google
    private bool _isGoogleLogin = false;
    //* Retour de la reponse d'Auth
    private AuthReturn? returnResponse;
    //*  Evite l'ouverture de 2 modals
    private bool _dialogOpen = false;

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
    
    protected override void OnInitialized()
    {
        Console.WriteLine("📦 ThermsDialog initialisé");
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
            returnResponse = await response.Content.ReadFromJsonAsync<AuthReturn>();
            
            //* Utilisation du ServiceUser pour l'enregistrer dans notre pipeline d'auth
            _userService.AuthenticateUser(returnResponse.User);
            
            //* Enregistrement dans le UserState
            var newUserState = new UserState
            {
                IdSession = returnResponse.IdSession,
                Id = returnResponse.User.Id,
                Email = returnResponse.User.Email,
                Name = returnResponse.User.Name,
                Role = returnResponse.User.Role,
                GivenName = returnResponse.User.GivenName,
                FamilyName = returnResponse.User.FamilyName,
                Picture = returnResponse.User.Picture,
                CreatedAt = returnResponse.User.CreatedAt,
                UpdatedAt = returnResponse.User.UpdatedAt
            };
                
            _userState.SetUser(newUserState);

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
            FullWidth = true,         
            MaxWidth = MaxWidth.Small,
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
                returnResponse = await response.Content.ReadFromJsonAsync<AuthReturn>();
                _userService.AuthenticateUser(returnResponse.User);

                var newUserState = new UserState
                {
                    IdSession = returnResponse.IdSession,
                    Id = returnResponse.User.Id,
                    Email = returnResponse.User.Email,
                    Name = returnResponse.User.Name,
                    Role = returnResponse.User.Role,
                    GivenName = returnResponse.User.GivenName,
                    FamilyName = returnResponse.User.FamilyName,
                    Picture = returnResponse.User.Picture,
                    CreatedAt = returnResponse.User.CreatedAt,
                    UpdatedAt = returnResponse.User.UpdatedAt
                };
                
                _userState.SetUser(newUserState);
                
                _navigationManager.NavigateTo("/dashboard");
            }
            
        }
        else
        {
            await OpenDialogAuth(user); // 💥 relance si fermé sans valider
        }

    }

    public async Task OpenDialogTherms()
    {
        if (_dialogOpen)
        {
            Console.WriteLine("⛔️ Modale déjà ouverte !");
            return;
        }

        _dialogOpen = true;
        Console.WriteLine("✅ Ouverture modale demandée");
            
        //* Options de la boîte de dialogue : fermeture sur Esc ou clic en dehors
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            FullWidth = true,         
            MaxWidth = MaxWidth.Small,
        };

        try
        {
            //* Affichage de la boîte de dialogue
            // await _dialogService.ShowAsync<ThermsDialog>("", options);
            var dialog = await _dialogService.ShowAsync<ThermsDialog>("", options);
            await dialog.Result;
            Console.WriteLine("FERMETURE");
            _dialogOpen = false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
