using System.Net;
using MaClasse.Client.Services;
using MaClasse.Client.States;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Menu;

public partial class ProfileDialog : ComponentBase
{
    private readonly IDialogService _dialogService;
    private readonly ServiceLogout _serviceLogout;
    private readonly NavigationManager _navigationManager;
    private readonly UserState _userState;
    private readonly ViewDashboardState _viewDashboardState;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ProfileDialog(
        UserState userState,
        ViewDashboardState viewDashboardState,
        HttpClient httpClient,
        IConfiguration configuration,
        IDialogService dialogService,
        ServiceLogout serviceLogout,
        NavigationManager navigationManager)
    {
        _userState = userState;
        _viewDashboardState = viewDashboardState;
        _httpClient = httpClient;
        _configuration = configuration;
        _dialogService = dialogService;
        _serviceLogout = serviceLogout;
        _navigationManager = navigationManager;
    }
    
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }
    
    public string Email => _userState.Email;
    public string Role => _userState.Role;
    public string Zone => _userState.Zone;
    public List<Rattachment> AsDirecteur => _userState.AsDirecteur;
    public List<Rattachment> AsProfesseur => _userState.AsProfesseur;
    public string AddDirecteurValue;
    public string DeleteDirecteurValue;
    public string AddProfesseurValue;
    public string DeleteProfesseurValue;

    public void ClosePolicy()
    {
        MudDialog?.Close(DialogResult.Ok(true));
    }

    private async void ChangeProfil()
    {
        //* Change le Role en base de donnée
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/auth/change-profil",
            new ChangeProfilRequest()
            {   
                Role = _userState.Role,
                Zone = _userState.Zone,
                IdSession = _userState.IdSession
            });

        if (response.IsSuccessStatusCode)
        {
            var user =  await response.Content.ReadFromJsonAsync<UserProfile>();
            _userState.Role = user.Role;
            _userState.Zone = user.Zone;
            _userState.UpdatedAt = user.UpdatedAt;
            StateHasChanged();
        }
    }

    private void SetZone(string zone)
    {
        _userState.Zone = zone;
    }
    
    private void SetRole(string role)
    {
        _userState.Role = role;
    }

    private async void AddRattachment()
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/auth/add-rattachment",
            new RattachmentRequest
            {   
                IdSession = _userState.IdSession,
                IdDirecteur = AddDirecteurValue,
                IdProfesseur = AddProfesseurValue
            });

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<List<Rattachment>>();
            
            _userState.SetAsDirecteur(result
                .Where(r => r.IdDirecteur == _userState.IdRole)
                .ToList());
            
            _userState.SetAsProfesseur(result
                .Where(r => r.IdProfesseur == _userState.IdRole)
                .ToList());
            
            //* Ouverture d'une popup avec le message de success
            var parameters = new DialogParameters
            {
                ["Message"] = "Rattachement effectué avec succès",
                ["Result"] = "Success"
            };  
            
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                CloseButton = true,
                MaxWidth = MaxWidth.ExtraSmall,
                FullWidth = true
            };
            
            //* Affichage de la boîte de dialogue
            var dialog = await _dialogService.ShowAsync<ErrorRattachmentDialog>("Succès du Rattachement", parameters, options);
            await dialog.Result;
            
            //* Mise a jour du bouton de choix des dahsboards
            _viewDashboardState.GetViewDashboardFromDatabase();
        }

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            //* Ouverture d'une popup avec le message d'erreur
            var message = await response.Content.ReadAsStringAsync();
            
            var parameters = new DialogParameters
            {
                ["Message"] = message,
                ["Result"] = "Error"
            };  
            
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                CloseButton = true,
                MaxWidth = MaxWidth.ExtraSmall,
                FullWidth = true
            };
            
            //* Affichage de la boîte de dialogue
            var dialog = await _dialogService.ShowAsync<ErrorRattachmentDialog>("Erreur de Rattachement", parameters, options);
            await dialog.Result;
        }
        
            ResetInput();
    }

    private async void DeleteRattachment()
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/auth/delete-rattachment",
            new RattachmentRequest
            {   
                IdSession = _userState.IdSession,
                IdDirecteur = DeleteDirecteurValue,
                IdProfesseur = DeleteProfesseurValue
            });

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<List<Rattachment>>();
            
            _userState.SetAsDirecteur(result
                .Where(r => r.IdDirecteur == _userState.IdRole)
                .ToList());
            
            _userState.SetAsProfesseur(result
                .Where(r => r.IdProfesseur == _userState.IdRole)
                .ToList());
            
            //* Ouverture d'une popup avec le message de success
            var parameters = new DialogParameters
            {
                ["Message"] = "Rattachement supprimé avec succès",
                ["Result"] = "Success"
            };
            
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                CloseButton = true,
                MaxWidth = MaxWidth.ExtraSmall,
                FullWidth = true
            };
            
            //* Affichage de la boîte de dialogue
            var dialog = await _dialogService.ShowAsync<ErrorRattachmentDialog>("Succès du Rattachement", parameters, options);
            await dialog.Result;
            
            //* Mise a jour du bouton de choix des dahsboards
            _viewDashboardState.GetViewDashboardFromDatabase();
        }

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            //* Ouverture d'une popup avec le message d'erreur
            var message = await response.Content.ReadAsStringAsync();

            var parameters = new DialogParameters
            {
                ["Message"] = message,
                ["Result"] = "Error"
            };

            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                CloseButton = true,
                MaxWidth = MaxWidth.ExtraSmall,
                FullWidth = true
            };

            //* Affichage de la boîte de dialogue
            var dialog =
                await _dialogService.ShowAsync<ErrorRattachmentDialog>("Erreur de Rattachement", parameters, options);
            await dialog.Result;
        }

            ResetInput();
    }

    private void ResetInput()
    {
        AddDirecteurValue = String.Empty;
        DeleteDirecteurValue = String.Empty;
        AddProfesseurValue = String.Empty;
        DeleteProfesseurValue = String.Empty;
        
        StateHasChanged();

    }

    private async Task DeleteUser()
    {
        //* ouverture d'une popup de confirmation
        var parameters = new DialogParameters
        {
            ["Message"] = "Êtes-vous sûr de vouloir supprimer votre compte ?",
        };
        
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            MaxWidth = MaxWidth.ExtraSmall,
            FullWidth = true
        };
        
        var dialog = await _dialogService.ShowAsync<ConfirmDeleteDialog>("Confirmation de suppression", parameters, options);
        
        var result = await dialog.Result;
        
        if (!result.Canceled)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_configuration["Url:ApiGateway"]}/api/auth/delete-user",
                new DeleteUserRequest
                {   
                    IdSession = _userState.IdSession,
                });

            if (response.IsSuccessStatusCode)
            {
                //* Avec reset des données user
                _serviceLogout.Logout(_userState.IdSession);
            
                //* ouverture d'une popup de succès et redirection vers la page de login
                var  dialogParameters= new DialogParameters
                {
                    ["Message"] = "Votre compte a été supprimé avec succès",
                    ["Result"] = "Success"

                };
                
                var dialogOptions = new DialogOptions
                {
                    CloseOnEscapeKey = true,
                    CloseButton = true,
                    MaxWidth = MaxWidth.Small,
                    FullWidth = true
                };
                
                var dialogSuccess = await _dialogService.ShowAsync<ErrorRattachmentDialog>("Succès de la suppression", dialogParameters, dialogOptions);
                await dialogSuccess.Result;
                
                _navigationManager.NavigateTo("/");
            }
        }
    }
}