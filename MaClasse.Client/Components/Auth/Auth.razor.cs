using MaClasse.Client.Services;
using MaClasse.Client.States;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using MudBlazor;

namespace MaClasse.Client.Components.Auth;

public partial class Auth : ComponentBase
{
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly IConfiguration _configuration;
    private readonly IJSRuntime _jsRuntime;
    private readonly ServiceAuthentication _serviceAuthentication;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserService _userService;
    private readonly UserState _userState;
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly RefreshService _refreshService;
    private readonly SchedulerState _schedulerState;
    private readonly ILogger<Auth> _logger;


    public Auth (
        NavigationManager navigationManager, 
        HttpClient httpClient,
        IDialogService dialogService,
        IConfiguration configuration,
        IJSRuntime jsRuntime,
        ServiceAuthentication serviceAuthentication,
        IHttpContextAccessor httpContextAccessor,
        UserService userService,
        UserState userState,
        ProtectedLocalStorage protectedLocalStorage,
        RefreshService refreshService,
        SchedulerState schedulerState,
        ILogger<Auth> logger)
    {
        _navigationManager = navigationManager;
        _httpClient = httpClient;
        _dialogService = dialogService;
        _configuration = configuration;
        _jsRuntime = jsRuntime;
        _serviceAuthentication = serviceAuthentication;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _userState = userState;
        _protectedLocalStorage = protectedLocalStorage;
        _refreshService = refreshService;
        _schedulerState = schedulerState;
        _logger = logger;
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
    
    [JSInvokable]
    public async Task ReceiveGoogleToken(string jwtToken)
    {
        _isGoogleLogin = true;
        StateHasChanged();

        var coucou = _configuration["Url:ApiGateway"];
        _logger.LogInformation("Url:ApiGateway", coucou);

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/auth/google-login",
            new GoogleTokenRequest{ Token = jwtToken });
        
            if (response.IsSuccessStatusCode)
            {
                returnResponse = await response.Content.ReadFromJsonAsync<AuthReturn>();
                
                //* Enregistrement dans le ProtectedLocalStorage
                await _protectedLocalStorage.SetAsync("MaClasseAuth", returnResponse.IdSession);
                
                //* Utilisation du ServiceUser pour l'enregistrer dans notre pipeline d'auth
                _userService.AuthenticateUser(returnResponse.UserWithRattachment.UserProfile);
                
                //* Enregistrement dans le UserState
                var newUserState = new UserState
                {
                    AccessToken = returnResponse.UserWithRattachment.AccessToken,
                    IdSession = returnResponse.IdSession,
                    Id = returnResponse.UserWithRattachment.UserProfile.Id,
                    IdRole = returnResponse.UserWithRattachment.UserProfile.IdRole,
                    Email = returnResponse.UserWithRattachment.UserProfile.Email,
                    Name = returnResponse.UserWithRattachment.UserProfile.Name,
                    Role = returnResponse.UserWithRattachment.UserProfile.Role,
                    Zone = returnResponse.UserWithRattachment.UserProfile.Zone,
                    GivenName = returnResponse.UserWithRattachment.UserProfile.GivenName,
                    FamilyName = returnResponse.UserWithRattachment.UserProfile.FamilyName,
                    Picture = returnResponse.UserWithRattachment.UserProfile.Picture,
                    CreatedAt = returnResponse.UserWithRattachment.UserProfile.CreatedAt,
                    UpdatedAt = returnResponse.UserWithRattachment.UserProfile.UpdatedAt,
                    AsDirecteur = returnResponse.UserWithRattachment.AsDirecteur,
                    AsProfesseur = returnResponse.UserWithRattachment.AsProfesseur
                    
                };

                var newSchedulerState = new SchedulerState
                {
                    IdScheduler = returnResponse.Scheduler.IdScheduler,
                    IdUser = returnResponse.Scheduler.IdUser,
                    Appointments = returnResponse.Scheduler.Appointments,
                    CreatedAt = returnResponse.Scheduler.CreatedAt,
                    UpdatedAt = returnResponse.Scheduler.UpdatedAt,
                    SchedulerDisplayed = returnResponse.Scheduler.IdUser
                };
                    
                _userState.SetUser(newUserState);
                _schedulerState.SetScheduler(newSchedulerState);
                

                //* Recherche si c'est un nouvel utilisateur, dans ce cas on ouvre la modal de complément d'infos
                if (returnResponse.IsNewUser)
                {
                    await OpenDialogAuth(returnResponse.UserWithRattachment.UserProfile);
                }
                else
                {
                    _navigationManager.NavigateTo("/dashboard");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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
        
        if (!result.Canceled && result.Data is SignupDialogResult dialogResult)
        {
            var payload = new SignupDialogResult
                { 
                    Role = dialogResult.Role,
                    Zone = dialogResult.Zone,
                    AccessToken = _userState.AccessToken,
                    
                };
            
            //* Requete vers le back pour compléter le profil
            var response = await _httpClient.PostAsJsonAsync(
                $"{_configuration["Url:ApiGateway"]}/api/auth/finished-signup", payload);

            if (response.IsSuccessStatusCode)
            {   
                returnResponse = await response.Content.ReadFromJsonAsync<AuthReturn>();
                _userService.AuthenticateUser(returnResponse.UserWithRattachment.UserProfile);

                var newUserState = new UserState
                {
                    AccessToken = returnResponse.UserWithRattachment.AccessToken,
                    IdSession = returnResponse.IdSession,
                    Id = returnResponse.UserWithRattachment.UserProfile.Id,
                    IdRole = returnResponse.UserWithRattachment.UserProfile.IdRole,
                    Email = returnResponse.UserWithRattachment.UserProfile.Email,
                    Name = returnResponse.UserWithRattachment.UserProfile.Name,
                    Role = returnResponse.UserWithRattachment.UserProfile.Role,
                    Zone = returnResponse.UserWithRattachment.UserProfile.Zone,
                    GivenName = returnResponse.UserWithRattachment.UserProfile.GivenName,
                    FamilyName = returnResponse.UserWithRattachment.UserProfile.FamilyName,
                    Picture = returnResponse.UserWithRattachment.UserProfile.Picture,
                    CreatedAt = returnResponse.UserWithRattachment.UserProfile.CreatedAt,
                    UpdatedAt = returnResponse.UserWithRattachment.UserProfile.UpdatedAt,
                    AsDirecteur = returnResponse.UserWithRattachment.AsDirecteur,
                    AsProfesseur = returnResponse.UserWithRattachment.AsProfesseur
                };
                
                var newSchedulerState = new SchedulerState
                {
                    IdScheduler = returnResponse.Scheduler.IdScheduler,
                    IdUser = returnResponse.Scheduler.IdUser,
                    Appointments = returnResponse.Scheduler.Appointments,
                    CreatedAt = returnResponse.Scheduler.CreatedAt,
                    UpdatedAt = returnResponse.Scheduler.UpdatedAt,
                    SchedulerDisplayed = returnResponse.Scheduler.IdUser

                };
                
                _userState.SetUser(newUserState);
                _schedulerState.SetScheduler(newSchedulerState);
                
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
            return;
        }

        _dialogOpen = true;
            
        //* Options de la boîte de dialogue : fermeture sur Esc ou clic en dehors
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            FullWidth = true,         
            MaxWidth = MaxWidth.Small,
        };
        
        //* Affichage de la boîte de dialogue
        var dialog = await _dialogService.ShowAsync<ThermsDialog>("", options);
        await dialog.Result;
        _dialogOpen = false;
    }
    
    public async Task OpenDialogPolicy()
    {
        if (_dialogOpen)
        {
            return;
        }

        _dialogOpen = true;
            
        //* Options de la boîte de dialogue : fermeture sur Esc ou clic en dehors
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            FullWidth = true,         
            MaxWidth = MaxWidth.Small,
        };
            //* Affichage de la boîte de dialogue
            var dialog = await _dialogService.ShowAsync<ProfileDialog>("", options);
            await dialog.Result;
            _dialogOpen = false;
    }
}
