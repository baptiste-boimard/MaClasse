using MaClasse.Client.Components.Errors;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;


namespace MaClasse.Client.Components.Auth;

public partial class Auth3 : ComponentBase
{
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;

    public Auth3 (
        NavigationManager navigationManager, 
        HttpClient httpClient,
        IDialogService dialogService
    )
    {
        _navigationManager = navigationManager;
        _httpClient = httpClient;
        _dialogService = dialogService;
    }
    
    private bool _isConnected = false;
    
    //* Variable de switch mode connexion ou inscription
    private bool _isLoginMode = true;
    //* Variable d'affiche du titre
    private string CurrentTitle => _isLoginMode ? "Bienvenue à nouveau" : "Créer un compte";
    //* Notre utilisateur
    private UserProfile? _profile;
    //* Gestion des erreurs de login/inscription
    private bool _error = false;
    private string _errorMessage;
    private bool _isDialogOpened = false;

    
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

            //* Obtenir l'URI absolu
            var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        
            //* Extraction des paramètres de la query string
            var query = QueryHelpers.ParseQuery(uri.Query);

            if (query.TryGetValue("error", out var boolParam)) bool.TryParse(boolParam, out _error);
        
            if (query.TryGetValue("message", out var stringParam)) _errorMessage = stringParam;

            if (_error && !_isDialogOpened)
            {
                _isDialogOpened = true;

                await OpenDialogError();
            }
        }
    }
    
    public void OnGet()
    {
        Console.WriteLine("coucou");
        var coucou = "coucou";
        // Vérifier si TempData contient la clé "Error" et la convertir en booléen
        if (TempData.ContainsKey("Error"))
        {
            // La conversion peut être nécessaire si la valeur est stockée en tant que string
            Error = Convert.ToBoolean(TempData["Error"]);
        }
        
        // Récupérer le message (ou string vide s'il n'existe pas)
        Message = TempData.ContainsKey("Message") ? TempData["Message"] as string : string.Empty;
    }

    private async Task OpenDialogError()
    {
        //* Paramètres à transmettre à la boîte de dialogue
        var parameters = new DialogParameters { { "Message", _errorMessage } };
            
        //* Options de la boîte de dialogue : fermeture sur Esc ou clic en dehors
        var options = new DialogOptions { CloseOnEscapeKey = true };
            
                
        //* Affichage de la boîte de dialogue
        var dialog = await _dialogService.ShowAsync<ErrorLoginDialog>("Erreur", parameters, options);
        
        // dialog.Close(); // Cela fermera manuellement le dialogue
        
        var result = await dialog.Result;  // Attendre la fermeture de la boîte
            
        if (result != null)
        {
            //* Après fermeture de la boîte, réinitialiser les variables d'erreur et de message

            _error = false;
            _errorMessage = string.Empty;
            
            StateHasChanged();
            
            _navigationManager.NavigateTo("https://localhost:7235", replace: true);


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
        var returnUrl = "https://localhost:7261/api/auth/login";
        var encodedReturnUrl = System.Net.WebUtility.UrlEncode(returnUrl);
        Nav.NavigateTo($"https://localhost:7261/api/auth/signin-google?returnUrl={encodedReturnUrl}", forceLoad: true);
    }

    private void GoogleSignupAction()
    {
        //* Lance le challenge Google pour inscription
        var returnUrl = "https://localhost:7261/api/auth/signup";
        var encodedReturnUrl = System.Net.WebUtility.UrlEncode(returnUrl);
        Nav.NavigateTo($"https://localhost:7261/api/auth/signin-google?returnUrl={encodedReturnUrl}", forceLoad: true);
    }
}