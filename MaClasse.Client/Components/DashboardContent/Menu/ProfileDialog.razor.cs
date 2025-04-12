using System.Net;
using MaClasse.Client.States;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Menu;

public partial class ProfileDialog : ComponentBase
{
    private readonly IDialogService _dialogService;
    private readonly UserState _userState;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ProfileDialog(
        UserState userState,
        HttpClient httpClient,
        IConfiguration configuration,
        IDialogService dialogService)
    {
        _userState = userState;
        _httpClient = httpClient;
        _configuration = configuration;
        _dialogService = dialogService;
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
            
            //* Ouverture d'une popup avec le message de succes
            var message = await response.Content.ReadAsStringAsync();
            
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
            
            //! Quand elle se ferme je reset le contenu de l'input
            
            ClosePolicy();
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
            
            //! Quand elle se ferme je reset le contenu de l'input
        }
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
            
        }
        
        //! Quand elle se ferme je reset le contenu de l'input
    }
}