using MaClasse.Client.States;
using MaClasse.Shared;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Menu;

public partial class ProfileDialog : ComponentBase
{
    private readonly UserState _userState;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ProfileDialog(
        UserState userState,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _userState = userState;
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }
    
    public string Email => _userState.Email;
    public string Role => _userState.Role;
    public string Zone => _userState.Zone;
    public List<Rattachment> AsDirecteur => _userState.AsDirecteur;
    public List<Rattachment> AsProfesseur => _userState.AsProfesseur;

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

    private void OnChipClicked(string role)
    {
        _userState.Role = role;
    }

    private void SetZone(string zone)
    {
        _userState.Zone = zone;
    }
    
    private void SetRole(string role)
    {
        _userState.Role = role;
    }
}